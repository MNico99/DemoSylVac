namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Tenaris.Library.Log;

    public partial class Model
    {
        private static readonly Lazy<Model> instance = new Lazy<Model>(() => new Model());
        private bool isActive = false;
        private DateTimeOffset startInspectionDateTime = DateTimeOffset.Now;
        private readonly object syncRoot = new object();

        private Model()
        {
            try
            {
                this.Activate();
            }
            catch (Exception ex)
            {
                Trace.Exception(ex, "Initializing Proxy.");
            }
        }

        ~Model()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoteStop()
        {
            this.Values = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoteStart()
        {
            this.Values = new List<MetterValue>();
        }

        /// <summary>
        /// Raised after the inspection starts.
        /// </summary>
        public event EventHandler<EventArgs> InspectionStarted;

        /// <summary>
        /// Raised after the inspection ends.
        /// </summary>
        public event EventHandler<EventArgs> InspectionStopped;

        /// <summary>
        /// Raised after the inspection ends.
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChaned;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> StartListening;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> StopListening;

        /// <summary>
        /// 
        /// </summary>
        public List<MetterValue> Values { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this.Values = new List<MetterValue>();
            if (!this.IsInInspection)
            {
                this.sylvacDevice.Open();
                this.sylvacDevice.DataChanged += new EventHandler<DataChangedEventArgs>(OnSylvacDataReceived);

                this.terminateEvent.Reset();
                this.workerThread = new Thread(this.Run);
                this.workerThread.SetApartmentState(ApartmentState.MTA);
                this.workerThread.Start();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (this.IsInInspection)
            {
                this.sylvacDevice.DataChanged -= this.OnSylvacDataReceived;

                this.terminateEvent.Set();
                this.workerThread.Join();
                this.workerThread = null;

                this.sylvacDevice.Close();
            }

            this.Values = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return isActive;
        }

        public void Uninitialize()
        {
            this.Deactivate();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInInspection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsListening { get; set; }
        
        protected void DoDataChanged(IEnumerable<MetterValue> items)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.DoUpdateData(items)));
        }

        protected void DoInspectionStopped()
        {
            if (InspectionStopped != null)
            {
                this.InspectionStopped(this, new EventArgs());
            }

            if (this.IsInInspection)
            {
                this.RemoteStop();
                this.IsInInspection = false;
            }
        }

        protected void DoInspectionStarted()
        {
            if (!this.IsInInspection)
            {
                this.RemoteStart();
                this.startInspectionDateTime = DateTimeOffset.Now;
                this.IsInInspection = true;
            }

            if (InspectionStarted != null)
            {
                this.InspectionStarted(this, new EventArgs());
            }
        }

        private void DoUpdateData(IEnumerable<MetterValue> items)
        {
            lock (syncRoot)
            {
                if (this.DataChaned != null && this.Values != null)
                {
                    this.Values.AddRange(items);
                    //var index = 0;
                    //this.Values.ForEach(p => p.Index = index++);
                    this.DataChaned(this, new DataChangedEventArgs(this.Values));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Model Instance { get { return instance.Value; } }
    }
}