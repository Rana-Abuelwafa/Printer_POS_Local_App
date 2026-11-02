using System.IO.Ports;

namespace ITravel_App.Models
{
    public class PosSettings
    {
        public string PortName { get; set; } = "";

        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public Parity Parity { get; set; }

        public StopBits StopBits { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }
    }
}
