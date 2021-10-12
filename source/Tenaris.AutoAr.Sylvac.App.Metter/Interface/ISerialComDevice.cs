namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.IO.Ports;
    using System.Text;

    public interface ISerialComDevice
    {
        int BaudRate { get; set; }

        int DataBits { get; set; }

        Handshake Handshake { get; set; }

        Parity Parity { get; set; }

        string PortName { get; set; }

        StopBits StopBits { get; set; }

        bool Open();

        bool Send(byte[] data);

        bool Send(string data);        

        void Close();

        event DataReceived DataReceived;
    }
}