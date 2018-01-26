using System;
using ClangSharp;
using CCodeFramework.Interfaces;
using CCodeTypes.Types;

namespace CCodeAnalyser.ClangInterface
{
    public class CTypeParser : ICTypeParser
    {
        IAstDataBase _DB;
        IFactory _Factory = null;
        public CTypeParser(IFactory factory)
        {
            _DB = factory.Fac_getDataBase();
            _Factory = factory;
        }
        public CElement CTypeParser_ParseDataType(ClangSharp.Type Type)
        {
            string md5 = CCodeFramework.Util.Util.ComputeMd5(Type);
            CElement element = _DB.AstDB_FindElement(md5);
            
           CElement dataType = null;
            if (element == null)
            {
                switch(Type.TypeKind)
                {
                    case ClangSharp.Type.Kind.Bool:
                    case ClangSharp.Type.Kind.CharU:
                    case ClangSharp.Type.Kind.UChar:
                    case ClangSharp.Type.Kind.Char16:
                    case ClangSharp.Type.Kind.Char32:
                    case ClangSharp.Type.Kind.UShort:
                    case ClangSharp.Type.Kind.UInt:
                    case ClangSharp.Type.Kind.ULong:
                    case ClangSharp.Type.Kind.ULongLong:
                    case ClangSharp.Type.Kind.UInt128:
                    case ClangSharp.Type.Kind.CharS:
                    case ClangSharp.Type.Kind.SChar:
                    case ClangSharp.Type.Kind.WChar:
                    case ClangSharp.Type.Kind.Short:
                    case ClangSharp.Type.Kind.Int:
                    case ClangSharp.Type.Kind.Long:
                    case ClangSharp.Type.Kind.LongLong:
                    case ClangSharp.Type.Kind.Int128:
                    case ClangSharp.Type.Kind.Float:
                    case ClangSharp.Type.Kind.Double:
                    case ClangSharp.Type.Kind.LongDouble:
                    case ClangSharp.Type.Kind.Void:
                        dataType = visitArithMeticType(Type);
                        break;
                    case ClangSharp.Type.Kind.FunctionProto:

                        dataType = _Factory.fac_getCParser().CParser_VisitFunctionCursor(Type.Declaration);
                        if(dataType == null)
                        {
                            dataType= visitFunctionprototype(Type);//if no declairation of the function is found then create it as a new DataType
                        }
                        break;
                    case ClangSharp.Type.Kind.Elaborated:
                        dataType = CTypeParser_ParseDataType(Type.Declaration.Type);
                        break;
                    case ClangSharp.Type.Kind.BlockPointer:
                    case ClangSharp.Type.Kind.Pointer:
                    case ClangSharp.Type.Kind.MemberPointer:
                    case ClangSharp.Type.Kind.LValueReference:
                        dataType = visitPointerType(Type);
                        break;
                    case ClangSharp.Type.Kind.Record:
                        dataType = visitStructOrUnionType(Type);
                        break;
                    case ClangSharp.Type.Kind.IncompleteArray:
                    case ClangSharp.Type.Kind.ConstantArray:
                        dataType = visitArrayType(Type);
                        break;

                    case ClangSharp.Type.Kind.Enum:
                        dataType = visitEnumType(Type);
                        break;
                    case ClangSharp.Type.Kind.Typedef:
                        dataType = visitTypeDefType(Type);
                        break;
                    case ClangSharp.Type.Kind.Unexposed:
                    case ClangSharp.Type.Kind.Invalid:
                        if(Type.Declaration.Type.TypeKind!= ClangSharp.Type.Kind.Invalid && Type.Declaration.Type.TypeKind != ClangSharp.Type.Kind.Unexposed)
                        {
                            dataType = CTypeParser_ParseDataType(Type.Declaration.Type);
                        }
                        else if (Type.Canonical.TypeKind != ClangSharp.Type.Kind.Invalid && Type.Canonical.TypeKind != ClangSharp.Type.Kind.Unexposed)
                        {
                            dataType = CTypeParser_ParseDataType(Type.Canonical);
                        }
                        if(dataType == null)
                        {
                            Console.Write("");
                        }
                        break;
                    default:
                        Console.Write("");
                        break;

                }
            }
            else
            {
                if(element is CDataType)
                {
                    dataType = element as CDataType;

                }
            }
            return dataType;

        }
        public CElement visitFunctionprototype(ClangSharp.Type type)
        {
            CDataType datatype = (CDataType)visitArithMeticType(type);
            datatype.Kind = DataTypeKind.FUNCTION_PROTOTYPE;
            datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString());
            return datatype;
        }
        public CElement visitArithMeticType(ClangSharp.Type type)
        {
            try
            {
                CDataType datatype = new CDataType();
                datatype.ID = CCodeFramework.Util.Util.ComputeMd5(type);
                datatype.ElementKind = ElementKind.DataType;
                datatype.Kind = DataTypeKind.ARITHMETIC_TYPE;
                _DB.AstDB_AddElement(datatype);
               
                datatype.Name = type.Spelling;
                if (type.IsConstQualifiedType)
                {
                    datatype.Qualifier = Qualifier.CONST;
                }
                else if (type.IsVolatileQualifiedType)
                {
                    datatype.Qualifier = Qualifier.VOLATILE;
                }
                return datatype;
            }
            catch(Exception err)
            {
                
                Console.WriteLine(err.ToString());
                return null;
            }
           
        }

        public CElement visitArrayType(ClangSharp.Type type)
        {
            try
            {
                CDataType datatype = (CDataType)visitArithMeticType(type);
                datatype.Kind = DataTypeKind.ARRAY_TYPE;
                
                datatype.Parent = CTypeParser_ParseDataType(type.ArrayElementType).ID;
                datatype.Multiplicity = (int)type.NumOfElements;
                datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString()+ datatype.Parent+ datatype.Multiplicity.ToString());
                return datatype;
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                return null;
            }
            
        }

        public CElement visitEnumType(ClangSharp.Type Type)
        {
           try
            {
                if (Type.Declaration.Kind != CursorKind.NoDeclFound)
                {
                    CDataType datatype = (CDataType)visitArithMeticType(Type);
                    datatype.Kind = DataTypeKind.ENUM_TYPE;
                    datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString());
                    foreach (ClangSharp.Cursor child in Type.Declaration.Children)
                    {
                        CElement literal = _Factory.fac_getCParser().CParser_VisitVariableCursor(child);
                        datatype.Children.Add(literal);
                        datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + literal.ID);
                    }
                    return datatype;
                }
                else
                {
                    return CTypeParser_ParseDataType(Type.Canonical);

                }

            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                return null;
            }

           
            
         
        }

        public CElement visitPointerType(ClangSharp.Type type)
        {
            try
            {
                if (type != null)
                {
                    CDataType datatype = (CDataType)visitArithMeticType(type);
                    if (datatype != null)
                    {
                        datatype.Kind = DataTypeKind.POINTER_TYPE;

                        if (type.Pointee.Canonical.TypeKind == ClangSharp.Type.Kind.FunctionProto)
                        {
                            datatype.Kind = DataTypeKind.FUNCTION_POINTER;
                        }
                        CElement elm = CTypeParser_ParseDataType(type.Pointee);
                        if(elm!= null)
                        {
                            datatype.Parent = elm.ID;
                        }
                        
                        datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString() + datatype.Parent);
                        return datatype;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                return null;
            }
           
        }

        public CElement visitStructOrUnionType(ClangSharp.Type type)
        {
            try
            {
                if (type.Declaration.Kind != CursorKind.NoDeclFound)
                {
                    CDataType datatype = (CDataType)visitArithMeticType(type);

                    if (type.Declaration.Kind == CursorKind.UnionDecl)
                    {
                        datatype.Kind = DataTypeKind.UNION_TYPE;
                    }
                    else if (type.Declaration.Kind == CursorKind.StructDecl)
                    {
                        datatype.Kind = DataTypeKind.STRUCT_TYPE;
                    }
                    datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString() );
                    foreach (Cursor child in type.Declaration.Children)
                    {
                        datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + child.Spelling+child.Type.Spelling);
                    }

                    foreach (Cursor child in type.Declaration.Children)
                    {
                        if (child.Kind == CursorKind.FieldDecl)
                        {
                            CElement attr = _Factory.fac_getCParser().CParser_VisitVariableCursor(child);
                            if (attr != null)
                            {
                                   datatype.Children.Add(attr);
                            }
                        }
                    }
                    return datatype;
                }
                else
                {
                    return CTypeParser_ParseDataType(type.Canonical);
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                return null;
            }
          
           

            
        }

        public CElement visitTypeDefType(ClangSharp.Type type)
        {
            CDataType datatype = (CDataType)visitArithMeticType(type);
            datatype.Kind = DataTypeKind.TYPEDEF_TYPE;
           
            if (type.Declaration.Kind != CursorKind.NoDeclFound)
            {
                datatype.Parent = CTypeParser_ParseDataType(type.Declaration.TypedefDeclUnderlyingType).ID;
            }
            else
            {
                datatype.Parent =  CTypeParser_ParseDataType(type.Canonical).ID;
            }
            datatype.ID = CCodeFramework.Util.Util.ComputeMd5ForString(datatype.ID + datatype.Kind.ToString()+ datatype.Parent);
            return datatype;
        }
    }
}
