using System;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tenaris.AutoAr.Sylvac.Library.Metter.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class VeearDeviceConfiguration : ConfigurationSection, IXmlSerializable
    {
        #region ViewConfiguration Properties
        /// <summary>
        /// 
        /// </summary>
        public static VeearDeviceConfiguration Settings
        {
            get
            {
                return (VeearDeviceConfiguration)ConfigurationManager.GetSection("VeearDeviceConfiguration");
            }
            set
            {
                ConfigurationManager.RefreshSection("VeearDeviceConfiguration");
            }
        }
        #endregion


        #region Configuration properties

        [ConfigurationProperty("Enabled", IsRequired = true, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["Enabled"];
            }
            set
            {
                base["Enabled"] = value;
            }
        }

        [ConfigurationProperty("Period", IsRequired = true, DefaultValue = 1000)]
        public int Period
        {
            get
            {
                return (int)base["Period"];
            }
            set
            {
                base["Period"] = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("BaudRate", IsRequired = true, DefaultValue = 4800)]
        public int BaudRate
        {
            get
            {
                return (int)base["BaudRate"];
            }
            set
            {
                base["BaudRate"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("DataBits", IsRequired = true, DefaultValue = 7)]
        public int DataBits
        {
            get
            {
                return (int)this["DataBits"];
            }
            set
            {
                this["DataBits"] = value;
            }
        }

        /// <summary>
        /// Handshake = System.IO.Ports.Handshake
        /// </summary>
        [ConfigurationProperty("Handshake", IsRequired = true, DefaultValue = System.IO.Ports.Handshake.None)]
        public System.IO.Ports.Handshake Handshake
        {
            get
            {

                var value = this["Handshake"].ToString();
                switch (value.ToUpper())
                {
                    case "XONXOFF": return System.IO.Ports.Handshake.XOnXOff;
                    case "REQUESTTOSEND": return System.IO.Ports.Handshake.RequestToSend;
                    case "REQUESTTOSENDXONXOFF": return System.IO.Ports.Handshake.RequestToSendXOnXOff;
                    default: return System.IO.Ports.Handshake.None;
                }
            }
            set
            {
                this["Handshake"] = value.ToString();
            }
        }

        /// <summary>
        ///                 //this.device.Parity = System.IO.Ports.Parity.Even;
        /// </summary>
        [ConfigurationProperty("Parity", IsRequired = true, DefaultValue = System.IO.Ports.Parity.Even)]
        public System.IO.Ports.Parity Parity
        {
            get
            {
                var value = this["Parity"].ToString();
                switch (value.ToUpper())
                {
                    case "EVEN": return System.IO.Ports.Parity.Even;
                    case "MARK": return System.IO.Ports.Parity.Mark;
                    case "ODD" : return System.IO.Ports.Parity.Odd;
                    case "SPACE": return System.IO.Ports.Parity.Space;
                    default: return System.IO.Ports.Parity.None;
                }
            }
            set
            {
                this["Parity"] = value.ToString();
            }
        }

        /// <summary>
        ///                 //this.device.StopBits = System.IO.Ports.StopBits.One;
        /// </summary>
        [ConfigurationProperty("StopBits", IsRequired = true, DefaultValue = System.IO.Ports.StopBits.One)]
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                var value = this["StopBits"].ToString();
                switch (value.ToUpper())
                {
                    case "ONE" : return System.IO.Ports.StopBits.One;
                    case "ONEPOINTFIVE" : return System.IO.Ports.StopBits.OnePointFive;
                    case "TWOO" : return System.IO.Ports.StopBits.Two;
                    default: return System.IO.Ports.StopBits.None;
                }
            }
            set
            {
                this["StopBits"] = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("PortName", IsRequired = true, DefaultValue = "COM1")]
        public string PortName
        {
            get
            {
                return this["PortName"].ToString();
            }
            set
            {
                this["PortName"] = value;
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