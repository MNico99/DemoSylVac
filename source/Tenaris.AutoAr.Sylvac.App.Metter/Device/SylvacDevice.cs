namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Threading;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Tenaris.Library.Log;

    /// <summary>
    /// Interfaces with a serial port. There should only be one instance
    /// of this class for each serial port to be used.
    /// </summary>
    public class SylvacDevice : SerialComDevice
    {
        public event EventHandler<DataChangedEventArgs> DataChanged;

        protected override void DoDataReceived(string value)
        {
            if (value.IndexOf((char)'\r') > -1)
            {
                var items = new List<MetterValue>();
                var values = value.Split((char)'\r', (char)'\t');

                for (int i = 0; i < values.Length; i++)
                {
                    var number = 0.0d;
                    if (values[i].Trim() != string.Empty)
                    {
                        var isNumeric = double.TryParse(values[i], out number);
                        if (isNumeric)
                        {
                            //var sampleDateTime = DateTimeOffset.Now.Subtract(this.startInspectionDateTime);
                            items.Add(new MetterValue() { Date = DateTimeOffset.Now, Index = 0, Value = number });
                        }
                        else
                            Trace.Debug("The value {0} is not a number", value[i]);
                    }
                }

                if (DataChanged != null)
                    this.DataChanged(this, new DataChangedEventArgs(items));
            }
        }
    }
}