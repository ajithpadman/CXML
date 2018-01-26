using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCodeTypes.Xml
{
    public abstract class IXmlDecorator : IXmlModel
    {
        protected IXmlModel _Parent;
      
        public IXmlDecorator(IXmlModel model)
        {
            _Parent = model;
        }

        public virtual void Deserialise(string CumlPath)
        {
            
        }

        public virtual void Serialise(string path)
        {
           
        }
    }
}
