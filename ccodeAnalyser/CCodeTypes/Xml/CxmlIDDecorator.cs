using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCodeTypes.Xml
{
    public class CxmlIDDecorator : IXmlDecorator
    {
        CXml _Model;
        public CxmlIDDecorator(CXml model) : base(model)
        {
            if(model!= null)
            {
                UpdateID(model);
            }
            
            _Model = model;
        }
        public override void Serialise(string path)
        {
            if(_Model!= null)
            {
                _Model.Serialise(path);
            }
            
        }
        public override void Deserialise(string CumlPath)
        {
            if(_Model!= null)
            {
                _Model.Deserialise(CumlPath);
            }
            
        }
        /// <summary>
        /// Update he IDs corresponding to correct properties
        /// </summary>
        /// <param name="model"></param>
        protected virtual void UpdateID(CXml model)
        {
            foreach(DataType type in model.DataTypes)
            {
                //calculate the new ID
                string newID = CalculateDataTypeID(type, model);
                //remap all references to old id with the new ID
                remapDataTypeNewID(newID, type.ID, model);
                //change the self ID to new ID
                type.ID = newID;
            }
            foreach(Function f in model.Functions)
            {
                string newID = CalculateOperatrionID(f, model);
                remmapOperationID(newID, f.ID, model);
                f.ID = newID;
            }
        }
        /// <summary>
        /// Check if a datatype is a reference in some way to the parent itself
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected bool findIfSelfReference(DataType parent,DataType data, CXml model)
        {
            bool result = false;
           
            if (data != null)
            {
                
                /*
                 * if the parent id is same as the dataType ID then 
                 * the are the same Types
                 */
                if (parent.ID != data.ID)
                {
                    //if the datatype pointed to by the attribute is not same as parent 
                    //and the attribute type is not primitive type then get the pointed datatype
                    //incase of pointer or underlyng datatype in case of Typedef
                    if (data.pointToIDref != "" && data.pointToIDref != parent.ID)
                    {
                        var types = model.DataTypes.Where(x => x.ID == data.pointToIDref).ToList();
                        data = null;
                        if (types != null)
                        {
                            if (types.Count() > 0)
                            {
                                data = types.ElementAt(0);
                                //analyse the pointed to data Type
                                result = findIfSelfReference(parent, data, model);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Type with ID " + data.pointToIDref + " Not found ");
                        }

                    }
                    else if (data.pointToIDref == parent.ID)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
        protected void remapDataTypeNewID(string newID,string oldID,CXml model)
        {
            var types = model.DataTypes.Where(x => x.pointToIDref == oldID).ToList();
            types.ForEach(x => x.pointToIDref = newID);
             types = model.DataTypes.Where(x => x.underlyingTypeIDref == oldID).ToList();
            types.ForEach(x => x.underlyingTypeIDref = newID);

            foreach(DataType t in model.DataTypes)
            {
                var attr = t.Attributes.Where(x => x.TypeIDref == oldID).ToList();
                attr.ForEach(x => x.TypeIDref = newID);
            }

            var variables = model.Variables.Where(x => x.TypeIDref == oldID).ToList();
            variables.ForEach(x => x.TypeIDref = newID);

            var functions = model.Functions.Where(x => x.ReturnIDRef == oldID).ToList();
            functions.ForEach(x => x.ReturnIDRef = newID);
            foreach(Function f in model.Functions)
            {
                var param = f.Parameters.Where(x => x.TypeIDref == oldID).ToList();
                param.ForEach(x => x.TypeIDref = newID);

            }
            
        }
        protected void remmapOperationID(string newID,string oldID,CXml model)
        {
            foreach (Function f in model.Functions)
            {
                var called = f.CalledFunctions.Where(x => x == oldID).ToList();
                called.ForEach(x => x = newID);

            }
        }
        /// <summary>
        /// Calculate the hash ID on a Datatype
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string CalculateDataTypeID(DataType type , CXml model)
        {
            string Properties = type.Name+type.Qualifier.ToString()+type.StorageClass ; 
            foreach(Variable v in type.Attributes)
            {
                
               
                var attrType = model.DataTypes.Where(x => x.ID == v.TypeIDref);
             
                if (attrType!= null)
                {
                    if(attrType.Count() > 0)
                    {
                        if (findIfSelfReference(type, attrType.ElementAt(0), model))
                        {
                            Properties += attrType.ElementAt(0).Name + v.Name;//if it is self referencial only take its name and type name
                        }
                        else
                        {
                            Properties += CalculateVariableID(v, model);
                        }
                       
                    }
                }

            }
            var pointTo = model.DataTypes.Where(x => x.ID == type.pointToIDref);
            if (pointTo != null)
            {
                foreach (DataType pontType in pointTo)
                {
                    Properties += CalculateDataTypeID(pontType, model);
                }
            }
            return Util.Util.ComputeMd5ForString(Properties);
            
        }
        /// <summary>
        /// calculate the HASH ID f a variable
        /// </summary>
        /// <param name="var"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string CalculateVariableID(Variable var,CXml model)
        {
            string Properties = var.Name;
            var type = model.DataTypes.Where(x => x.ID == var.TypeIDref);
            if (type != null)
            {
                if (type.Count() > 0)
                {
                    Properties += CalculateDataTypeID(type.ElementAt(0), model);
                }
            }
            return Util.Util.ComputeMd5ForString(Properties);
        }
        /// <summary>
        /// Calculate the Hash ID of an operation
        /// </summary>
        /// <param name="op"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string CalculateOperatrionID(Function op,CXml model)
        {
            string Properties = op.Name+op.StorageClass+op.IsDefinition.ToString();
            var type = model.DataTypes.Where(x => x.ID == op.ReturnIDRef);
            if (type != null)
            {
                if (type.Count() > 0)
                {
                    Properties += CalculateDataTypeID(type.ElementAt(0), model);
                }
            }
            foreach(Variable v in op.Parameters)
            {
                Properties += CalculateVariableID(v, model);
            }
            foreach(string cfString in op.CalledFunctions)
            {
                
                    var calledList = model.Functions.Where(x => x.ID == cfString);
                    if (calledList != null)
                    {
                        Function cf = calledList.ElementAt(0);
                        if (cf != null)
                        {
                            Properties += cf.Name;
                            var cfret = model.DataTypes.Where(x => x.ID == cf.ReturnIDRef);
                            if (cfret != null)
                            {
                                if (cfret.Count() > 0)
                                {
                                    Properties += cfret.ElementAt(0).Name;
                                }
                            }
                            foreach (Variable v in cf.Parameters)
                            {
                                Properties += CalculateVariableID(v, model);
                            }
                        }
                    }
               
            }
            return Util.Util.ComputeMd5ForString(Properties);
        }
        
    }
}
