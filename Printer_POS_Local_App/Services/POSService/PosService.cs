
using System.Net.Sockets;
using System.Text;

namespace ITravel_App.Services.POSService
{
    public class PosService : IPosService
    {
        private readonly IConfiguration _config;

        public PosService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> ConnectionTest()
        {
            var ip = _config["Pos:Ip"];
            var port = int.Parse(_config["Pos:Port"]);

            using var client = new TcpClient();

            await client.ConnectAsync(ip, port);

            using var stream = client.GetStream();

            string requestXml =
    @"<CONNTOPOS>
    <ConnTest>Data</ConnTest>
</CONNTOPOS>";

            byte[] requestBytes = Encoding.UTF8.GetBytes(requestXml);

            await stream.WriteAsync(requestBytes);

            var buffer = new byte[4096];

            int bytesRead = await stream.ReadAsync(buffer);

            string responseXml =
                Encoding.UTF8.GetString(buffer, 0, bytesRead);

            return responseXml;
        }
    }
}
