using System;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tenaris.AutoAr.Sylvac.Library.Metter.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewConfiguration : ConfigurationSection, IXmlSerializable
    {
        #region ViewConfiguration Properties
        /// <summary>
        /// 
        /// </summary>
        public static ViewConfiguration Settings
        {
            get
            {
                return (ViewConfiguration)ConfigurationManager.GetSection("ViewConfiguration");
            }
            set
            {
                ConfigurationManager.RefreshSection("ViewConfiguration");
            }
        }
        #endregion


        #region Configuration properties

        [ConfigurationProperty("RejectMax", IsRequired = true, DefaultValue = 12.0)]
        public double RejectMax
        {
            get
            {
                return (double)base["RejectMax"];
            }
            set
            {
                base["RejectMax"] = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("RejectMin", IsRequired = true, DefaultValue = 2.0)]
        public double RejectMin
        {
            get
            {
                return (double)base["RejectMin"];
            }
            set
            {
                base["RejectMin"] = value;
            }
        }
        
        #endregion

        #region IXmlSerializable members
        /// <summary>
        /// 
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            DeserializeElement(reader, false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            return;
        }
        #endregion
    }
}