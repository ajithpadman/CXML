using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CCodeTypes.Xml
{
    public enum SWCType
    {
        SWC,
        LIB
    }
    [XmlRoot("SWCConfig")]
    public class SWCConfig
    {
        private List<SWC> _swc = new List<SWC>();

        [XmlElement("SWC", typeof(SWC))]
        public List<SWC> SWC
        {
            get { return _swc; }
            set { _swc = value; }
        }
    }
    [Serializable]
    public class SWC
    {
        [XmlAttribute("Parent")]
        public string Parent
        {
            get; set;
        }
        [XmlAttribute("Name")]
        public string Name
        {
            get; set;
        }
        [XmlAttribute("Type")]
        public SWCType Type
        {
            get; set;
        }

    }
}
