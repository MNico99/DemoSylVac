namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class DataChangedEventArgs : EventArgs
    {
        public IEnumerable<MetterValue> Values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public DataChangedEventArgs(IEnumerable<MetterValue> values)
        {
            this.Values = values;
        }
    }
}
