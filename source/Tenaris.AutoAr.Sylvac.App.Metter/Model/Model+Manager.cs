namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Tenaris.Library.Log;

    public partial class Model
    {
        private bool disposed;
        private int readPeriod;
        private Thread workerThread;
        private readonly ManualResetEvent terminateEvent = new ManualResetEvent(true);
        private readonly object thisLock = new object();
        private VeearDevice veearDevice;
        private SylvacDevice sylvacDevice;
        private float[] demoValues = new float[] { 0.5f, 1, 1.5f, 2, 2.4f, 3.1001f, 5.3123456f, 6, 6.2111223f, 7.3f, 7.7f, 8, 8.4f, 9.32213f, 13.5f, 10, 9, 8.555f, 7.3323f, 4, 5.6f, 4.3f, 3.8f, 2.89654f, 2.3f, 1.3f };
        private int index = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        //private void StartDevices()
        private void Activate()
        {
            try
            {
                this.DoStopListening();

                this.sylvacDevice = new SylvacDevice();
                this.sylvacDevice.BaudRate = SylvacDeviceConfiguration.Settings.BaudRate;
                this.sylvacDevice.DataBits = SylvacDeviceConfiguration.Settings.DataBits;
                this.sylvacDevice.Handshake = SylvacDeviceConfiguration.Settings.Handshake;
                this.sylvacDevice.Parity = SylvacDeviceConfiguration.Settings.Parity;
                this.sylvacDevice.PortName = SylvacDeviceConfiguration.Settings.PortName;
                this.sylvacDevice.StopBits = SylvacDeviceConfiguration.Settings.StopBits;

                if (VeearDeviceConfiguration.Settings.Enabled)
                {
                    this.veearDevice = new VeearDevice();
                    this.veearDevice.BaudRate = VeearDeviceConfiguration.Settings.BaudRate;
                    this.veearDevice.DataBits = VeearDeviceConfiguration.Settings.DataBits;
                    this.veearDevice.Handshake = VeearDeviceConfiguration.Settings.Handshake;
                    this.veearDevice.Parity = VeearDeviceConfiguration.Settings.Parity;
                    this.veearDevice.PortName = VeearDeviceConfiguration.Settings.PortName;
                    this.veearDevice.StopBits = VeearDeviceConfiguration.Settings.StopBits;

                    this.veearDevice.Open();
                    this.veearDevice.StartOrder += new EventHandler<EventArgs>(OnStartOrder);
                    this.veearDevice.StopOrder += new EventHandler<EventArgs>(OnStopOrder);
                    this.veearDevice.StartListening += new EventHandler<EventArgs>(OnStartListening);
                    this.veearDevice.StopListening += new EventHandler<EventArgs>(OnStopListening);
                }
                this.readPeriod = SylvacDeviceConfiguration.Settings.Period;
            }
            catch (Exception ex)
            {
                Trace.Exception(ex, true);
            }

            Trace.Message("Initializing manager");
        }

        private void Deactivate()
        {
            this.sylvacDevice.Close();

            this.veearDevice.StartOrder -= this.OnStartOrder;
            this.veearDevice.StopOrder -= this.OnStopOrder;
            this.veearDevice.StartListening -= this.OnStartListening;
            this.veearDevice.StopListening -= this.OnStopListening;

            this.veearDevice.Close();
        }

        private void OnStopOrder(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.Stop()));
        }

        private void OnStartOrder(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.Start()));
        }

        private void OnStartListening(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.DoStartListening()));
        }

        private void OnStopListening(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.DoStopListening()));
        }

        private void OnSylvacDataReceived(object sender, DataChangedEventArgs e)
        {
            var sampleDateTime = DateTimeOffset.Now.Subtract(this.startInspectionDateTime);
            foreach (var item in e.Values)
            {
                item.Index = sampleDateTime.TotalSeconds;
            }
            this.DoDataChanged(e.Values);
        }

        private void DoStopListening()
        {
            this.IsListening = false;

            if (StopListening != null)
            {
                this.StopListening(this, new EventArgs());
            }
        }

        private void DoStartListening()
        {
            this.IsListening = true;

            if (StartListening != null)
            {
                this.StartListening(this, new EventArgs());
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            this.disposed = true;
        }

        private void Run()
        {
            try
            {
                this.DoInspectionStarted();
                this.index = 0;
                this.startInspectionDateTime = DateTimeOffset.Now;
                while (!terminateEvent.WaitOne(readPeriod))
                {
                    try
                    {
                        var items = new List<MetterValue>();
                        items.Add(new MetterValue() { Date = DateTimeOffset.Now, Index = this.index, Value = demoValues[index % demoValues.Count()] });
                        this.DoDataChanged(items);
                        this.index++;
                        //this.sylvacDevice.Send(string.Format("CHA{0}?#{1}", 1, (char)'\r'));
                    }
                    catch (Exception e)
                    {
                        Trace.Exception(e, true);
                    }
                }
                this.DoInspectionStopped();
            }
            catch (Exception e)
            {
                Trace.Exception(e, true);
            }
        }

        
    }
}