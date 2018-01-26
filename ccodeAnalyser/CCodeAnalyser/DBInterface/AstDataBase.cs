using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCodeFramework.Interfaces;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CCodeTypes.Types;
using CCodeTypes.Xml;

namespace CCodeAnalyser.DBInterface
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    public class AstDataBase : IAstDataBase
    {
        Dictionary<string, CElement> ElementMap = new Dictionary<string, CElement>();
      
        IFactory _factory = null;
        public AstDataBase(IFactory factory)
        {
            _factory = factory;
          
        }
        public void AstDB_AddElement( CElement Element)
        {
            if(ElementMap.ContainsKey(Element.ID) == false)
            {
                ElementMap.Add(Element.ID, Element);
            }
        }

        public CElement AstDB_FindAndAddElement( CElement Element)
        {
            CElement element = AstDB_FindElement(Element.ID);
            if(element == null)
            {
                AstDB_AddElement(element);
               
            }
            return element;

        }

        public CElement AstDB_FindElement(string ElementID)
        {
           if(ElementMap.ContainsKey(ElementID))
            {
                return ElementMap[ElementID];
            }
           else
            {
                return null;
            }
        }
        
        public CXml PublishDataBase(string FilePath)
        {
            CXml Cxml = new CXml();
            XmlSerializer serializer = new XmlSerializer(typeof(CXml));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            ElementMap.Values.OrderBy(x => x.File);
            foreach (string key in ElementMap.Keys)
            {
                CElement element = ElementMap[key];
                if(element is CFunction)
                {
                    CFunction function = element as CFunction;
                    //if (function.StorageClass != StorageClass.Static)
                    {
                        Function f = _factory.Fac_GetXmlFunctionType(function);
                        if (function.IsDefinition == false)
                        {
                            IEnumerable<CElement> definition = ElementMap.Values.Where(x => x.ElementID == function.ElementID).ToList();
                            foreach (CElement e in definition)
                            {
                                if (e is CFunction)
                                {
                                    CFunction def = e as CFunction;
                                    if (def.IsDefinition == true)
                                    {
                                        f.Definition = def.ID;
                                    }
                                }
                            }
                        }
                        f.PrototypeID = function.ElementID;
                        f.Component = _factory.Fac_GetComponentName(function);
                        f.Interface = _factory.Fac_GetInterfaceName(function);
                        f.CalledFunctions.AddRange(function.Children);
                        Cxml.Functions.Add(f);
                    }

                }
                else if (element is CDataType)
                {
                    Cxml.DataTypes.Add(_factory.Fac_GetXmlDataType(element as CDataType));
                }
                else if (element is CVariable)
                {
                    CVariable cv = element as CVariable;
                    if(cv.VariableType == VariableType.GlobalVariable)
                    {
                        Variable v = _factory.Fac_GetXmlVariableType(element as CVariable);
                        v.Component = _factory.Fac_GetComponentName(element);
                        Cxml.Variables.Add(v);
                    }
                    

                }
            }
            try
            {
                using (var sww = new Utf8StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    settings.Encoding = Encoding.ASCII;
                    using (XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, Cxml, ns);
                        string xml = sww.ToString();
                        StreamWriter w = new StreamWriter(new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8);
                        w.Write(xml);
                        w.Close();
                    }
                }
            }

            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
            return Cxml;

        }
    

    }
}
