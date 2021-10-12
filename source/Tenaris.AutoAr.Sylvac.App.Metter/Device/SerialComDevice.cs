namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO.Ports;

    public abstract class SerialComDevice : ISerialComDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public event DataReceived DataReceived;

        /// <summary>
        /// Serial port class
        /// </summary>
        protected SerialPort serialPort = new SerialPort();

        /// <summary>
        /// BaudRate set to default for Serial Port Class
        /// </summary>
        protected int baudRate = 9600;

        /// <summary>
        /// DataBits set to default for Serial Port Class
        /// </summary>
        protected int dataBits = 8;

        /// <summary>
        /// Handshake set to default for Serial Port Class
        /// </summary>
        protected Handshake handshake = Handshake.None;

        /// <summary>
        /// Parity set to default for Serial Port Class
        /// </summary>
        protected Parity parity = Parity.None;

        /// <summary>
        /// Communication Port name, not default in SerialPort. Defaulted to COM1
        /// </summary>
        protected string portName = "COM1";

        /// <summary>
        /// StopBits set to default for Serial Port Class
        /// </summary>
        protected StopBits stopBits = StopBits.One;

        /// <summary>
        /// Holds data received until we get a terminator.
        /// </summary>
        protected string tString = string.Empty;

        /// <summary>
        /// Gets or sets BaudRate (Default: 9600)
        /// </summary>
        public int BaudRate { get { return this.baudRate; } set { this.baudRate = value; } }

        /// <summary>
        /// Gets or sets DataBits (Default: 8)
        /// </summary>
        public int DataBits { get { return this.dataBits; } set { this.dataBits = value; } }

        /// <summary>
        /// Gets or sets Handshake (Default: None)
        /// </summary>
        public Handshake Handshake { get { return this.handshake; } set { this.handshake = value; } }

        /// <summary>
        /// Gets or sets Parity (Default: None)
        /// </summary>
        public Parity Parity { get { return this.parity; } set { this.parity = value; } }

        /// <summary>
        /// Gets or sets PortName (Default: COM1)
        /// </summary>
        public string PortName { get { return this.portName; } set { this.portName = value; } }

        /// <summary>
        /// Gets or sets StopBits (Default: One}
        /// </summary>
        public StopBits StopBits { get { return this.stopBits; } set { this.stopBits = value; } }

        public bool Send(byte[] data)
        {
            try
            {
                serialPort.Write(data, 0, data.Length);
            }
            catch { return false; }
            return true;
        }

        public bool Send(string data)
        {
            try
            {
                data += (char)'\r';
                serialPort.Write(data);
            }
            catch { return false; }
            return true;
        }

        public void Close()
        {
            serialPort.DataReceived -= OnDataReceived;
            serialPort.Close();
        }

        /// <summary>
        /// Sets the current settings for the Comport and tries to open it.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool Open()
        {
            try
            {
                this.serialPort.BaudRate = this.baudRate;
                this.serialPort.DataBits = this.dataBits;
                this.serialPort.Handshake = this.handshake;
                this.serialPort.Parity = this.parity;
                this.serialPort.PortName = this.portName;
                this.serialPort.StopBits = this.stopBits;
                this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.OnDataReceived);
                this.serialPort.Open();
            }
            catch
            {
                return false;
            }
            try { serialPort.DtrEnable = true; }
            catch { }
            try { serialPort.RtsEnable = true; }
            catch { }

            return true;
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //byte[] buffer = new byte[this.serialPort.ReadBufferSize];
            //var bytesRead = this.serialPort.Read(buffer, 0, buffer.Length);
            //this.tString = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //if (this.tString.IndexOf((char)'\r') > -1)
            //{
            //    var workingString = this.tString;

            //    this.DoDataReceived(workingString);
            //}

            byte[] buffer = new byte[this.serialPort.ReadBufferSize];
            var bytesRead = this.serialPort.Read(buffer, 0, buffer.Length);
            var workingString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            this.DoDataReceived(workingString);
        }

        protected abstract void DoDataReceived(string value);

        //protected void DoDataReceived(string value)
        //{
        //    if (this.DataReceived != null)
        //    {
        //        var args = new SerialPortEventArgs(value);
        //        this.DataReceived(this, args);
        //    }
        //}
    }
}