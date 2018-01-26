using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCodeTypes.Xml
{
    
    public interface IXmlModel
    {
        void Serialise(string path);

        void Deserialise(string CumlPath);
    }
}
