using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printer_POS_Local_App.Models.POS;
using Printer_POS_Local_App.Services.POSService;
using System.IO.Ports;

namespace Printer_POS_Local_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly IPosCommunicationService _posService;
        public POSController(
          IPosCommunicationService posService)
        {
            _posService = posService;
        }
        [HttpGet("ports")]
        public IActionResult Ports()
        {
            return Ok(System.IO.Ports.SerialPort.GetPortNames());
        }
        [HttpGet("scan")]
        public async Task<IActionResult> ScanPorts()
        {
            var results = new List<object>();

            foreach (var portName in SerialPort.GetPortNames().Distinct())
            {
                try
                {
                    using var port = new SerialPort(
                        portName,
                        9600,
                        Parity.None,
                        8,
                        StopBits.One);

                    port.ReadTimeout = 3000;
                    port.WriteTimeout = 3000;

                    port.Open();

                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();

                    string xml =
        @"<CONNTOPOS>
<ConnTest>800</ConnTest>
</CONNTOPOS>";

                    port.Write(xml);

                    await Task.Delay(3000);

                    string response = port.ReadExisting();

                    results.Add(new
                    {
                        Port = portName,
                        Response = response
                    });

                    port.Close();
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        Port = portName,
                        Error = ex.Message
                    });
                }
            }

            return Ok(results);
        }

        [HttpPost("payment")]
        public async Task<IActionResult> Payment(
     PosPaymentRequest request)
        {
            var result =
                await _posService.ProcessPaymentAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

