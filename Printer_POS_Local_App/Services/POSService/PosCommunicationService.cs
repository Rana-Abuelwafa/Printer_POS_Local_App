using ITravel_App.Models;
using Microsoft.Extensions.Options;
using System.IO.Ports;
using System.Text;

namespace Printer_POS_Local_App.Services.POSService
{
    using Printer_POS_Local_App.Models.POS;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Text;
    using System.Xml.Serialization;

    namespace YourProject.Services.POS
    {
        public class PosCommunicationService : IPosCommunicationService, IDisposable
        {
            private readonly SerialPort _serialPort;
            private readonly ILogger<PosCommunicationService> _logger;
            private readonly SemaphoreSlim _lock = new(1, 1);

            public PosCommunicationService(
                IConfiguration configuration,
                ILogger<PosCommunicationService> logger)
            {
                _logger = logger;

                string portName = configuration["PosSettings:PortName"] ?? "COM4";
                int baudRate = int.Parse(configuration["PosSettings:BaudRate"] ?? "9600");

                _serialPort = new SerialPort
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Encoding = Encoding.ASCII,
                    ReadTimeout = 120000,
                    WriteTimeout = 10000
                    //ReadTimeout = 150000,
                    //WriteTimeout = 150000,

                };
                //_serialPort = new SerialPort(
                //    "COM3",
                //    9600,
                //    Parity.None,
                //    8,
                //    StopBits.One,
                //    ReadTimeout = 120000,
                //WriteTimeout = 10000);

                OpenPort();
            }

            private void OpenPort()
            {
                try
                {
                    if (!_serialPort.IsOpen)
                    {
                        _serialPort.Open();

                        _logger.LogInformation(
                            "POS Port Opened : {Port}",
                            _serialPort.PortName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Unable to open POS port");
                }
            }

            public bool IsConnected()
            {
                return _serialPort.IsOpen;
            }

            //public async Task<string> ConnectionTestAsync()
            //{
            //    //string xml =
            //    //    @"<CONNTOPOS><ConnTest>800</ConnTest></CONNTOPOS>";
            //    string xml =
            //      @"<TOPOS><TrxType>00</TrxType><Amount>1000</Amount></TOPOS>";
            //    return await SendAsync(xml);
            //}


            public async Task<string> SendAsync(string xml, string expectedClosingTag)
            {
                await _lock.WaitAsync();

                try
                {
                    // Open port if needed
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        OpenPort();
                    }

                    // Clear previous communication
                    //_serialPort.DiscardInBuffer();
                    //_serialPort.DiscardOutBuffer();
                    if (_serialPort.BytesToRead > 0)
                    {
                        var pending = _serialPort.ReadExisting();

                        _logger.LogWarning(
                            "Pending POS data before request: {Data}",
                            pending);
                    }
                    _logger.LogInformation("========== POS REQUEST ==========");
                    _logger.LogInformation(xml);

                    // If the bank later confirms CRLF is required,
                    // replace Write() with:
                    // _serialPort.Write(xml + "\r\n");
                   // _serialPort.Write(xml);
                    var bytes = Encoding.ASCII.GetBytes(xml);

                    _logger.LogInformation(
                        "POS SEND TEXT: {Xml}",
                        xml);

                    _logger.LogInformation(
                        "POS SEND HEX: {Hex}",
                        BitConverter.ToString(bytes));

                    _logger.LogInformation(
                        "POS BEFORE WRITE: Open={Open}, Encoding={Encoding}, BytesToRead={BytesToRead}",
                        _serialPort.IsOpen,
                        _serialPort.Encoding.WebName,
                        _serialPort.BytesToRead);

                    _serialPort.Write(bytes, 0, bytes.Length);

                    _logger.LogInformation(
                        "POS AFTER WRITE: Open={Open}, BytesToRead={BytesToRead}",
                        _serialPort.IsOpen,
                        _serialPort.BytesToRead);

                    var response = await ReadResponseAsync(expectedClosingTag);

                    _logger.LogInformation("========== POS RESPONSE ==========");
                    _logger.LogInformation(response);

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        throw new TimeoutException("POS returned an empty response.");
                    }

                    return response;
                }
                catch (TimeoutException)
                {
                    _logger.LogError("Timeout waiting for POS response.");

                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error communicating with POS.");

                    // Try reopening the port if it was disconnected
                    try
                    {
                        if (_serialPort != null)
                        {
                            if (_serialPort.IsOpen)
                                _serialPort.Close();

                            OpenPort();
                        }
                    }
                    catch
                    {
                        // Ignore reconnect errors and rethrow original exception
                    }

                    throw;
                }
                finally
                {
                    _lock.Release();
                }
            }

            private async Task<string> ReadResponseAsync(string expectedClosingTag)
            {
                var responseBuilder = new StringBuilder();
                var stopwatch = Stopwatch.StartNew();
                _logger.LogInformation("==========start count ==========");
                while (stopwatch.Elapsed < TimeSpan.FromSeconds(150))
                {
                    try
                    {
                        string chunk = _serialPort.ReadExisting();
             
                       // _logger.LogInformation(" Chunk: {Chunk}", chunk);
                        if (!string.IsNullOrEmpty(chunk))
                        {
                            _logger.LogInformation("POS Received Chunk: {Chunk}", chunk);

                            responseBuilder.Append(chunk);

                            string response = responseBuilder.ToString();

                            if (response.Contains(expectedClosingTag, StringComparison.OrdinalIgnoreCase))
                            {
                                return response;
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Ignore and continue waiting
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading data from POS.");
                        throw;
                    }

                    await Task.Delay(100);
                    await Task.Delay(500);

                    _logger.LogInformation(
                        "After connection test: IsOpen={IsOpen}, BytesToRead={BytesToRead}",
                        _serialPort.IsOpen,
                        _serialPort.BytesToRead);
                }

                string partialResponse = responseBuilder.ToString();

                _logger.LogWarning(
                    "POS response timeout. Partial response: {Response}",
                    partialResponse);

                throw new TimeoutException(
                    $"Timeout waiting for POS response. Partial response: {partialResponse}");
            }

            public void Dispose()
            {
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }

                    _serialPort.Dispose();
                }
                catch
                {
                }
            }

            public async Task<PosPaymentResponse> ProcessPaymentAsync(PosPaymentRequest request)
            {
                _logger.LogInformation(
              "POS CONFIG: Port={Port}, Baud={Baud}, DataBits={DataBits}, Parity={Parity}, StopBits={StopBits}, Handshake={Handshake}, Dtr={Dtr}, Rts={Rts}, Encoding={Encoding}, CodePage={CodePage}",
              _serialPort.PortName,
              _serialPort.BaudRate,
              _serialPort.DataBits,
              _serialPort.Parity,
              _serialPort.StopBits,
              _serialPort.Handshake,
              _serialPort.DtrEnable,
              _serialPort.RtsEnable,
              _serialPort.Encoding.WebName,
              _serialPort.Encoding.CodePage
          );
                try
                {
                    // ===========================
                    // Step 1 - Connection Test
                    // ===========================
 //const string connectionXml = @"<CONNTOPOS><ConnTest>800</ConnTest></CONNTOPOS>";

                    //var connectionResponse = await SendAsync(
                    //    connectionXml,
                    //    "</CONNFROMPOS>");

                    //if (string.IsNullOrWhiteSpace(connectionResponse))
                    //{
                    //    return new PosPaymentResponse
                    //    {
                    //        Success = false,
                    //        Message = "Connection test failed. No response received from POS."
                    //    };
                    //}

                    //if (!connectionResponse.Contains("<CONNFROMPOS>"))
                    //{
                    //    return new PosPaymentResponse
                    //    {
                    //        Success = false,
                    //        Message = "Invalid connection response received from POS.",
                    //        RawResponse = connectionResponse
                    //    };
                    //}
                   

                    // ===========================
                    // Step 2 - Build Payment XML
                    // ===========================

                    string amount = FormatAmount(request.Amount);

                    string paymentXml =
                            $@"<TOPOS><TrxType>{request.TrxType ?? ""}</TrxType><Amount>{amount}</Amount><RefNum>{request.RefNum ?? ""}</RefNum><RcptNum>{request.ReceiptNum ?? ""}</RcptNum></TOPOS>";

                    // ===========================
                    // Step 3 - Send Payment
                    // ===========================
                    //_logger.LogInformation(
                    //    "Before payment: IsOpen={IsOpen}, BytesToRead={BytesToRead}",
                    //    _serialPort.IsOpen,
                    //    _serialPort.BytesToRead);

                    var paymentResponse = await SendAsync(
                        paymentXml,
                        "</FROMPOS>");

                    if (string.IsNullOrWhiteSpace(paymentResponse))
                    {
                        return new PosPaymentResponse
                        {
                            Success = false,
                            Message = "Timeout waiting for POS payment response."
                        };
                    }

                    // ===========================
                    // Step 4 - Parse XML
                    // ===========================

                    var receipt = ParseReceipt(paymentResponse);

                    if (receipt == null)
                    {
                        return new PosPaymentResponse
                        {
                            Success = false,
                            Message = "Unable to parse POS response.",
                            RawResponse = paymentResponse
                        };
                    }

                    // ===========================
                    // Step 5 - Check Result
                    // ===========================

                    bool approved = receipt.ResponseCode == "0";

                    return new PosPaymentResponse
                    {
                        Success = approved,
                        Message = approved
                            ? receipt.TransactionStatus ?? "Approved"
                            : receipt.TransactionStatus ?? "Payment Declined",

                        Receipt = receipt,
                        RawResponse = paymentResponse
                    };
                }
                catch (TimeoutException)
                {
                    return new PosPaymentResponse
                    {
                        Success = false,
                        Message = "Timeout waiting for POS response."
                    };
                }
                catch (Exception ex)
                {
                    return new PosPaymentResponse
                    {
                        Success = false,
                        Message = ex.Message
                    };
                }
            }
            private static string FormatAmount(decimal amount)
            {
                return Math.Round(amount * 100, 0, MidpointRounding.AwayFromZero)
                           .ToString("0");
            }
            private PosReceipt ParseReceipt(string xml)
            {
                var serializer = new XmlSerializer(typeof(PosReceipt));

                using var reader = new StringReader(xml);

                return (PosReceipt)serializer.Deserialize(reader)!;
            }
        }
    }
}
