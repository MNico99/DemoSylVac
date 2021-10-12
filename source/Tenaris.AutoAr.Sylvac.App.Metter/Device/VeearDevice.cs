namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Linq;
    using System.Text;
    using System.IO.Ports;
    using System.Collections.Generic;

    using System.Windows;
    using System.Windows.Threading;

    using Tenaris.Library.Log;

    /// <summary>
    /// Interfaces with a serial port. There should only be one instance
    /// of this class for each serial port to be used.
    /// </summary>
    public class VeearDevice : SerialComDevice
    {
        private const string wordSet = "iB";
        //private const string wordSet = "dC";

        public event EventHandler<EventArgs> StartOrder;
        public event EventHandler<EventArgs> StopOrder;
        public event EventHandler<EventArgs> StartListening;
        public event EventHandler<EventArgs> StopListening;

        public override bool Open()
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

                //this.serialPort.Write("b");
                //this.serialPort.Write("x");
                //this.serialPort.Write("lE");
                ////this.serialPort.Write("lA");
                //this.serialPort.Write("kA");
                //System.Threading.Thread.Sleep(1000);
                //this.serialPort.Write(wordSet);

                this.serialPort.Write("b");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write("x");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write(" ");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write("l");
                this.serialPort.Write("E");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write("o");
                this.serialPort.Write("A");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write("k");
                this.serialPort.Write("B");
                System.Threading.Thread.Sleep(1000);
                this.serialPort.Write("i");
                this.serialPort.Write("B");
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

        protected override void DoDataReceived(string value)
        {
            try
            {
                if (value != string.Empty)
                {
                    var response = value[0];
                    switch (response)
                    {
                        case 'o':
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.Send(" ")));
                            if (this.StartListening != null)
                                this.StartListening(this, new EventArgs());
                            break;
                        case 'x': break;
                        case 'r':
                        case 's':
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.Send(" ")));
                            if (this.StartListening != null)
                                this.StartListening(this, new EventArgs());
                            break;
                        case 't':
                            Restart();
                            break;
                        case 'e':                            
                            Restart();
                            break;
                        case 'i':
                            Restart();
                            break;
                        case 'v':
                            Restart();
                            break;
                        default:
                            var str = response.ToString();
                            var index = System.Text.Encoding.UTF8.GetBytes(str)[0] - System.Text.Encoding.UTF8.GetBytes("A")[0];
                            switch (index)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 4:
                                case 5:
                                case 7:
                                    Restart();
                                    break;
                                case 3:
                                    if (this.StartOrder != null)
                                        this.StartOrder(this, new EventArgs());
                                    Restart();
                                    break;
                                case 6:
                                    if (this.StopOrder != null)
                                        this.StopOrder(this, new EventArgs());
                                    Restart();
                                    break;
                                default:
                                    //Restart();
                                    Trace.Debug("VeearDataReceived DEFAULT {0}", value);
                                    break;                                    
                            }
                            break;
                    }
                    Trace.Debug("VeearDataReceived {0}", value);
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(ex, true);
            }
        }

        void Restart() 
        {
            if (this.StopListening != null)
                this.StopListening(this, new EventArgs());
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.Send(wordSet)));
            //if (this.StartListening != null)
                //this.StartListening(this, new EventArgs());
        }


        private bool Send(string data)
        {
            try
            {
                data += (char)'\r';
                serialPort.Write(data);
            }
            catch { return false; }
            return true;
        }
    }
}