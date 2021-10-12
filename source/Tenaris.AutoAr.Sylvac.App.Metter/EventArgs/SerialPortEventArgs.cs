namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public delegate void DataReceived(object sender, SerialPortEventArgs arg);

    public class SerialPortEventArgs : EventArgs
    {
        public string ReceivedData { get; private set; }
        public SerialPortEventArgs(string data)
        {
            ReceivedData = data;
        }
    }
}