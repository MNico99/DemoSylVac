namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Tenaris.Library.Log;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.IO;
    using Microsoft.Win32;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using Tenaris.AutoAr.Sylvac.App.Metter.Model;

    public partial class Model
    {
        private static readonly Lazy<Model> instance = new Lazy<Model>(() => new Model());
        private bool isActive = false;
        private DateTimeOffset startInspectionDateTime = DateTimeOffset.Now;
        private readonly object syncRoot = new object();
        private static string path;
        private bool isLoading = false;
        public static List<MetterValue> valuesExcel;

        SqlConnection SqlConnection;
        SqlCommand SqlCommand;


        private Model()
        {
            try
            {
                SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
                SqlCommand = new SqlCommand();
                SqlCommand.Connection = SqlConnection;
                SqlCommand.CommandType = CommandType.StoredProcedure;
                this.Activate();

                
            }
            catch (Exception ex)
            {
                Trace.Exception(ex, "Initializing Proxy.");
            }
        }

        public void Add()
        {
            bool IsAdded = false;
            try
            {
                index = 0;
                while(index < valuesExcel.Count())
                {
                    SqlCommand.Parameters.Clear();
                    SqlCommand.CommandText = "ADD_Values";
                    SqlCommand.Parameters.AddWithValue("xCoord", valuesExcel[index].Value);
                    SqlCommand.Parameters.AddWithValue("yCoord", valuesExcel[index].Index);


                    SqlConnection.Open();
                    int NoOfRowsAffected = SqlCommand.ExecuteNonQuery();
                    IsAdded = NoOfRowsAffected > 0;
                    index++;
                    SqlConnection.Close();
                }
                
            }
            catch (SqlException ex)
            {

                throw ex;
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

        public event EventHandler<EventArgs> LoadingStopped;

        public event EventHandler<EventArgs> LoadingStarted;

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
            if (!this.IsInInspection && isLoading == false )
            {
                this.sylvacDevice.Open();
                this.sylvacDevice.DataChanged += new EventHandler<DataChangedEventArgs>(OnSylvacDataReceived);

                this.terminateEvent.Reset();
                this.workerThread = new Thread(this.Run);
                this.workerThread.SetApartmentState(ApartmentState.MTA);
                this.workerThread.Start();
            }
            else if(!this.IsInLoading && isLoading == true)
            {
                this.isLoading = true;
                this.Values = new List<MetterValue>();
                if (!this.IsInLoading)
                {
                    Stream checkStream = null;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = false;
                    openFileDialog.Filter = "Excel Files |*.xls;*.xlsx;*.xlsm; |All files (*.*)|*.*";

                    if ((bool)openFileDialog.ShowDialog())
                    {
                        try
                        {
                            if ((checkStream = openFileDialog.OpenFile()) != null)
                            {
                                //TODO
                                path = openFileDialog.FileName;
                                this.terminateEvent.Reset();
                                this.workerThread = new Thread(this.Run);
                                this.workerThread.SetApartmentState(ApartmentState.MTA);
                                this.workerThread.Start();                                

                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// 

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
            if(this.IsInLoading)
            {

                this.terminateEvent.Set();
                this.workerThread.Join();
                this.workerThread = null;
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

        public bool IsLoading(bool isLoading)
        {
            return this.isLoading = isLoading;
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
        public bool IsInLoading { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsListening { get; set; }
        
        protected void DoDataChanged(IEnumerable<MetterValue> items)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => this.DoUpdateData(items)));
        }

        protected void DoLoadingStarted()
        {
            if(!this.IsInLoading)
            {
                this.RemoteStart();
                this.IsInLoading = true;
            }
            if(LoadingStopped != null)
            {
                this.LoadingStarted(this, new EventArgs());
            }
            
        }

        protected void DoLoadingStopped()
        {
            if (LoadingStopped != null)
            {
                this.LoadingStopped(this, new EventArgs());
            }

            if (this.IsInLoading)
            {
                this.RemoteStop();
                this.IsInLoading = false;
                this.isLoading = false;
            }
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