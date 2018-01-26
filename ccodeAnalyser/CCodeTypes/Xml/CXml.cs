using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CCodeTypes.Xml
{
    [XmlRoot("CXml")]
    public class CXml:IXmlModel
    {
      
        private List<DataType> _DataTypes = new List<DataType>();
        private List<Variable> _Variables = new List<Variable>();
        private List<Function> _Functions = new List<Function>();
       

        [XmlArray("Operations")]
        [XmlArrayItem("Operation", typeof(Function))]
        public List<Function> Functions
        {
            get { return _Functions; }
            set { _Functions.AddRange(value); }
        }

        [XmlArray("DataTypes")]
        [XmlArrayItem("DataType", typeof(DataType))]
        public List<DataType> DataTypes
        {
            get { return _DataTypes; }
            set { _DataTypes.AddRange(value); }
        }

        [XmlArray("Variables")]
        [XmlArrayItem("Variable", typeof(Variable))]
        public List<Variable> Variables
        {
            get { return _Variables; }
            set { _Variables.AddRange(value); }
        }
        public void Serialise(string path)
        {
           
            XmlSerializer serializer = new XmlSerializer(typeof(CXml));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            try
            {
                using (var sww = new Utf8StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    settings.Encoding = new UTF8Encoding(false);
                    using (XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, this, ns);
                        string xml = sww.ToString();
                        StreamWriter w = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.ReadWrite), new UTF8Encoding(false));
                        w.Write(xml);
                        w.Close();
                    }
                }

            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

        }

        public void Deserialise(string CumlPath)
        {
            CXml uml;
            XmlSerializer serializer = new XmlSerializer(typeof(CXml));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            try
            {
                FileStream fs = new FileStream(CumlPath, FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                uml = (CXml)serializer.Deserialize(reader);
                this.Variables.AddRange(uml.Variables);
                this.Functions.AddRange(uml.Functions);
                this.DataTypes.AddRange(uml.DataTypes);
                fs.Close();
            }

            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

        }
    }

    
    
    public class Function
    {
        private List<Variable> _parameters = new List<Variable>();
        private List<string> _called = new List<string>();
        [XmlAttribute("Name")]
        public  string Name
        {
            get;set;
        }
        [XmlAttribute("Component")]
        public string Component
        {
            get;set;
        }
        [XmlAttribute("Interface")]
        public string Interface
        {
            get; set;
        }

        [XmlElement("ReturnTypeIDRef")]
        public string ReturnIDRef
        {
            get;set;
        }
        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter",typeof(Variable))]
        public List<Variable> Parameters
        {
            get { return _parameters; }
            set { _parameters.AddRange(value); }
        }

        [XmlArray("CalledFunctions")]
        [XmlArrayItem("FunctionIDref")]
        public List<string> CalledFunctions
        {
            get { return _called; }
            set { _called.AddRange(value); }
        }

        [XmlElement("ID")]
        public string ID
        {
            get;set;
        }
        [XmlIgnore]
        public string PrototypeID
        {
            get;set;
        }
        [XmlElement("Definition")]
        public string Definition
        {
            get; set;
        }
        [XmlElement("IsDefinition")]
        public bool IsDefinition
        {
            get;set;
        }
        [XmlElement("StorageClass")]
        public string StorageClass
        {
            get;set;
        }
        [XmlElement("File")]
        public string File
        {
            get;set;
        }
        [XmlElement("Line")]
        public string Line
        {
            get;set;
        }
        [XmlElement("Column")]
        public string Column
        {
            get;set;
        }
        bool _isAdded = false;
       [XmlIgnore]

        public bool IsAdded
        {
            get { return _isAdded; }
            set { _isAdded = value; }
        }

    }
    public class DataType
    {
        private List<Variable> _Children = new List<Variable>();

        [XmlElement("Kind")]
        public string Kind
        {
            get;set;
        }
        [XmlElement("ID")]
        public string ID
        {
            get;
            set;
        }
        [XmlAttribute("Name")]
        public string Name
        {
            get; set;
        }
        [XmlElement("File")]
        public string File
        {
            get; set;
        }
        [XmlElement("Line")]
        public string Line
        {
            get; set;
        }
        [XmlElement("Column")]
        public string Column
        {
            get; set;
        }
        [XmlElement("underlyingTypeIDref")]
        public string underlyingTypeIDref
        {
            get;set;
        }
        [XmlElement("pointToIDref")]
        public string pointToIDref
        {
            get;set;
        }
        [XmlElement("StorageClass")]
        public string StorageClass
        {
            get; set;
        }
        [XmlElement("Qualifier")]
        public string Qualifier
        {
            get; set;
        }
        [XmlArray("Atrributes")]
        [XmlArrayItem("Attribute", typeof(Variable))]
        public List<Variable> Attributes
        {
            get { return _Children; }
            set { _Children.AddRange(value); }
        }



    }
    public class Variable
    {
        [XmlElement("ID")]
        public string ID
        {
            get;
            set;
        }
        [XmlAttribute("Name")]
        public string Name
        {
            get; set;
        }
        [XmlElement("File")]
        public string File
        {
            get; set;
        }
        [XmlElement("Line")]
        public string Line
        {
            get; set;
        }
        [XmlElement("Column")]
        public string Column
        {
            get; set;
        }
        [XmlElement("TypeIDref")]
        public string TypeIDref
        {
            get; set;
        }
        [XmlAttribute("Component")]
        public string Component
        {
            get; set;
        }
    }
    
}
