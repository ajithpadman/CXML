using System.Linq;
using ClangSharp;
using CCodeFramework.Interfaces;
using CCodeTypes.Types;
using System;

namespace CCodeAnalyser.ClangInterface
{
    public class CParser : ICParser
    {
        IAstDataBase _DB;
        ICTypeParser _typeParser;

        public CParser(IFactory factory)
        {
            _DB = factory.Fac_getDataBase();
            _typeParser = factory.fac_getTypeParser();
        }
        /// <summary>
        /// Analyse all function prototypes
        /// </summary>
        /// <param name="Cursor"></param>
        /// <returns></returns>
        public CElement CParser_VisitFunctionCursor(Cursor Cursor)
        {
            //first compute the md5 of the cursor and check if it already exist in database
            //if yes return the already created object
            string md5 = CCodeFramework.Util.Util.ComputeMd5(Cursor);
            CFunction function;
            CElement element = _DB.AstDB_FindElement(md5);
            if (element == null && Cursor.Kind != CursorKind.NoDeclFound)
            {
                
                function = new CFunction();
                function.ID = md5;
                string fileName = Cursor.Location.File.Name.Replace('/', '\\');
                function.File = fileName;
                function.Line = Cursor.Location.Line;
                function.Column = Cursor.Location.Column;
                function.ElementID = CCodeFramework.Util.Util.ComputeElementMd5(Cursor);
                function.ElementKind = ElementKind.Function;
                function.Name = Cursor.Spelling;
                function.ProtoTypeHash = CCodeFramework.Util.Util.ComputeMd5(Cursor.ResultType);
                _DB.AstDB_AddElement(function);//add the function object back to database
                CElement retur = _typeParser.CTypeParser_ParseDataType(Cursor.ResultType);//find the datatype of the return Type of the function
                function.ReturnType = retur.ID;
                function.IsDefinition = Cursor.IsDefinition;
                
                function.StorageClass = (CCodeTypes.Types.StorageClass)Cursor.StorageClass;
                int argCount = Cursor.NumArguments;
                for (uint i = 0; i < argCount; i++)//iterate through all the arguments and find their name and type
                {
                    ClangSharp.Cursor cur = Cursor.GetArgument(i);
                    function.ProtoTypeHash += CCodeFramework.Util.Util.ComputeMd5(cur.Type);
                    function.Parameters.Add(CParser_VisitVariableCursor(cur));
                }
                var CalledFunctions = Cursor.Descendants.Where(x => x.Kind == CursorKind.CallExpr);//find all functions called from this function
                foreach(Cursor called in CalledFunctions)
                {
                    if(called.Referenced != null)//Referenced contain the original function cursor
                    {
                        if( called.Referenced.Location.File.Name!= function.File&& called.Referenced.Kind == CursorKind.FunctionDecl)//analyse only those functions which are not defined in the same file
                        {
                            function.Children.Add(CParser_VisitFunctionCursor(called.Referenced).ID);
                        }
                    
                    }
                    
                }
                return function;

            }
            else
            {
                if (element is CFunction)//if already found in database check if that is of type Function
                {
                    function = element as CFunction;
                    return function;
                }
                else
                {
                    return null;
                }
                
            }

        }
        /// <summary>
        /// Function to analyse the Variables 
        /// they can be either global variable or Function arguments 
        /// or Struct Union member fileds or Enumeration types
        /// </summary>
        /// <param name="Cursor"></param>
        /// <returns></returns>
        public CElement CParser_VisitVariableCursor(Cursor Cursor)
        {
            //first compute the md5 of the cursor and check if it already exist in database
            //if yes return the already created object
            string md5 = CCodeFramework.Util.Util.ComputeMd5(Cursor);
            CElement element = _DB.AstDB_FindElement(md5);
            CVariable variable = null;
            try
            {
                if (null == element)
                {
                    variable = new CVariable();
                    variable.Name = Cursor.Spelling;
                    variable.ID = md5;
                    variable.ElementID = CCodeFramework.Util.Util.ComputeElementMd5(Cursor);
                    variable.File = Cursor.Location.File.Name.Replace('/', '\\');
                    variable.Line = Cursor.Location.Line;
                    variable.Column = Cursor.Location.Column;
                    variable.ElementKind = ElementKind.Variable;
                    if (Cursor.Kind == CursorKind.VarDecl)
                    {
                        variable.VariableType = VariableType.GlobalVariable;//if it is a variable declairation then it must be a global variable
                    }
                    else if (Cursor.Kind == CursorKind.ParmDecl)
                    {
                        variable.VariableType = VariableType.FunctionParameter;//if it is a paramdecl from a function it is a parameter
                    }
                    else if (Cursor.Kind == CursorKind.FieldDecl)
                    {
                        variable.VariableType = VariableType.MemberField;// memeber fields in structure or union
                    }
                    else if (Cursor.Kind == CursorKind.EnumConstantDecl)
                    {
                        variable.VariableType = VariableType.EnumLiteral;//Enumeration literals
                    }
                    _DB.AstDB_AddElement(variable);//add variable to database
                    CElement type = _typeParser.CTypeParser_ParseDataType(Cursor.Type);
                    if(type!= null)
                    {

                        variable.Type = type.ID;//assign the md5 as the type
                    }
                    else
                    {
                        variable.Type = "";
                    }
                    

                    return variable;
                }
                else
                {
                    if (element is CVariable)//return the Database element only if it is of type Variable
                    {
                        variable = element as CVariable;
                        return variable;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                return null;
            }
        }

    }
}
