namespace Tenaris.AutoAr.Sylvac.Library.Metter.ViewModel
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
    using Microsoft.Practices.Prism.ViewModel;

    using Tenaris.AutoAr.Sylvac.Library.Metter.Model;
    using System.ComponentModel;
    using System.IO;
    using Microsoft.Win32;

    public class ViewModel : NotificationObject, INotifyPropertyChanged
    {

        private readonly DelegateCommand windowClosing;
        private readonly DelegateCommand startCommand;
        private readonly DelegateCommand stopCommand;
        private readonly DelegateCommand loadCommand;
        private readonly DelegateCommand saveCommand;
        private double rejectMax = ViewConfiguration.Settings.RejectMax;
        private double rejectMin = ViewConfiguration.Settings.RejectMin;

        public ObservableCollection<MetterValue> Values
        {
            get { return Model.Instance.Values == null ? null : new ObservableCollection<MetterValue>(Model.Instance.Values.Where(p => p.Value > this.rejectMin && p.Value < this.rejectMax)); }
            //get { return Model.Instance.Values == null ? null : new ObservableCollection<MetterValue>(Model.Instance.Values); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ThresholdMax { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double ThresholdMin { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double LastValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset StopDateTime { get; set; }

        //private bool isInInspection;
        /// <summary>
        /// 
        /// </summary>
        public bool IsInInspection
        {
            get
            {
                return Model.Instance.IsInInspection;
                //return this.isInInspection;
            }
            set
            {
                //this.isInInspection = value;
                this.RaisePropertyChanged(() => this.IsInInspection);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return Model.Instance.IsLoaded;
            
            }
            set
            {
                this.RaisePropertyChanged(() => this.IsLoaded);
            }
        }


        public bool IsListening
        {
            get
            {
                return Model.Instance.IsListening;
            }
        }

        

        public ICommand WindowClosing { get { return this.windowClosing; } }

        /// <summary>
        /// 
        /// </summary>
        public ICommand StartCommand { get { return this.startCommand; } }

        /// <summary>
        /// 
        /// </summary>
        public ICommand StopCommand { get { return this.stopCommand; } }

        public ICommand LoadCommand { get { return this.loadCommand; } }

        public ICommand SaveCommand { get { return this.saveCommand; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool isListeningMic = false;

        public bool IsListeningMic
        {
            get { return isListeningMic; }
            set { 
                    isListeningMic = value;
                    OnPropertyChanged("IsListeningMic");
                    Tenaris.Library.Log.Trace.Debug("IsListeningMic: {0}", value);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) 
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public ViewModel()
        {
            this.windowClosing = new DelegateCommand(this.DoWindowClosing);
            this.startCommand = new DelegateCommand(this.DoStartAcquisition, this.CanStartAcquisition);
            this.stopCommand = new DelegateCommand(this.DoStopAcquisition, this.CanStopAcquisition);
            this.loadCommand = new DelegateCommand(this.DoLoadAcquisition, this.CanLoadAcquistion);
            this.saveCommand = new DelegateCommand(this.DoSaveAcquisition, this.CanSaveAcquisition);

            //this.isInInspection = false;

            Model.Instance.InspectionStarted += new EventHandler<EventArgs>(OnInspectionStarted);
            Model.Instance.InspectionStopped += new EventHandler<EventArgs>(OnInspectionStopped);
            Model.Instance.StartListening += new EventHandler<EventArgs>(OnStartListening);
            Model.Instance.StopListening += new EventHandler<EventArgs>(OnStopListening);
            Model.Instance.DataChaned += new EventHandler<DataChangedEventArgs>(OnDataChaned);
        }

        private bool CanLoadAcquistion()
        {
            return !this.IsLoaded;
        }

        private void DoSaveAcquisition()
        {
            //TODO: Guardar los datos del archivo en la BD
        }

        private bool CanSaveAcquisition()
        {
            //TODO
            return this.IsLoaded;
        }
        private void DoLoadAcquisition()
        {
            Stream checkStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if((checkStream = openFileDialog.OpenFile()) != null)
                    {
                        //TODO
                        this.saveCommand.RaiseCanExecuteChanged();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private void OnStopListening(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.IsListening);
            this.IsListeningMic = !this.IsListening;
        }

        private void OnStartListening(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.IsListening);
            this.IsListeningMic = !this.IsListening;
        }

        private void OnDataChaned(object sender, DataChangedEventArgs e)
        {
            var items = e.Values.Where(p => p.Value > this.rejectMin && p.Value < this.rejectMax);
            this.ThresholdMax = items.Count() > 0 ? items.Max(p => p.Value) : 0;
            this.ThresholdMin = items.Count() > 0 ? items.Min(p => p.Value) : 0;
            var item = items.LastOrDefault();
            this.LastValue = item == null ? 0 : item.Value;

            this.RaisePropertyChanged(() => this.ThresholdMax);
            this.RaisePropertyChanged(() => this.ThresholdMin);
            this.RaisePropertyChanged(() => this.LastValue);
            this.RaisePropertyChanged(() => this.Values);

            OnPropertyChanged("ThresholdMax");
            OnPropertyChanged("ThresholdMin");
            OnPropertyChanged("LastValue");
            OnPropertyChanged("Values");

        }

        private void OnInspectionStopped(object sender, EventArgs e)
        {
            this.IsInInspection = false;
            this.StartDateTime = DateTimeOffset.Now;
            this.RaisePropertyChanged(() => this.StartDateTime);
            OnPropertyChanged("StartDateTime");
            this.startCommand.RaiseCanExecuteChanged();
            this.stopCommand.RaiseCanExecuteChanged();
            this.RaisePropertyChanged(() => this.IsInInspection);
            OnPropertyChanged("IsInInspection");
        }

        private void OnInspectionStarted(object sender, EventArgs e)
        {
            this.IsInInspection = true;
            this.StopDateTime = DateTimeOffset.Now;
            this.RaisePropertyChanged(() => this.StartDateTime);
            OnPropertyChanged("StartDateTime");
            this.startCommand.RaiseCanExecuteChanged();
            this.stopCommand.RaiseCanExecuteChanged();
            this.RaisePropertyChanged(() => this.IsInInspection);
            OnPropertyChanged("IsInInspection");
            
        }

        private void DoWindowClosing()
        {
            Model.Instance.InspectionStarted -= this.OnInspectionStarted;
            Model.Instance.InspectionStopped -= this.OnInspectionStopped;
            Model.Instance.DataChaned -= this.OnDataChaned;

            Model.Instance.Uninitialize();
        }

        private void DoStartAcquisition()
        {
            Model.Instance.Start();
        }

        private bool CanStartAcquisition()
        {
            return !this.IsInInspection;
        }

        private void DoStopAcquisition()
        {
            Model.Instance.Stop();
        }

        private bool CanStopAcquisition()
        {
            return this.IsInInspection;
        }

    }
}