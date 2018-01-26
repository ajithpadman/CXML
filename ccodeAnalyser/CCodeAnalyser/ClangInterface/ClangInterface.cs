using System;
using CCodeFramework.Interfaces;

using ClangSharp;
using System.Security.Cryptography;
using CCodeTypes.Types;
using CCodeFramework;
namespace CCodeAnalyser.ClangInterface
{

    public class ClangInterface : IClang,IDisposable
    {
        ProjectModel _Model;
        Index _Index;
        ICParser _Parser;
        TranslationUnit _CurrentTu;

        

      

        public ClangInterface(IFactory factory)
        {
            _Index = new Index(true, true);
            _Parser = factory.fac_getCParser();
         
            


        }

        /// <summary>
        /// Method to parse functions in a File
        /// </summary>
        /// <param name="File">path to the C header file or Source File</param>
        public void Clang_FindFunctions(string File)
        {

            if(_Model != null)
            {
                //create the Translation unitwith the Compiler options passed to the CCodeAnalyser
                _CurrentTu = _Index.CreateTranslationUnit(File, _Model.ClangOptions.ToArray(), null, TranslationUnitFlags.DetailedPreprocessingRecord);
             
                if (_CurrentTu!=null)
                {
                    if(_CurrentTu.Cursor!=null)//check if the root C element is not null
                    {
                        
                        foreach (Cursor c in _CurrentTu.Cursor.Descendants)//Analyse all descendants for the root cursor
                        {
                           
                            //if the location of the cursor is  in the same file and is of type FunctionDecl then only call the parser
                            if (c.Kind == CursorKind.FunctionDecl && c.Location.File .Name == File)
                            {
                                _Parser.CParser_VisitFunctionCursor(c);
                               
                            
                            }
                        }
                    }
                    
                }
                _CurrentTu.Dispose();//Call Garbage collector as the Object is too huge
            }
            


        }
        /// <summary>
        /// Function to Find all the global variables in the file
        /// </summary>
        /// <param name="File"></param>
        public void Clang_FindGlobalVariable(string File)
        {
            if (_Model != null)
            {
                //Create Translation Unit
                _CurrentTu = _Index.CreateTranslationUnit(File, _Model.ClangOptions.ToArray(), null, TranslationUnitFlags.DetailedPreprocessingRecord);
                if (_CurrentTu != null)
                {
                    if (_CurrentTu.Cursor != null)
                    {
                        //Analyse only the Direct children on the Root Cursor as it is not possible to 
                        //Declare the Global variables as subsequent children
                        foreach (Cursor c in _CurrentTu.Cursor.Children)
                        {
                            //if the location of the cursor is  in the same file and is of type Variable Declairation then only call the parser
                            if (c.Kind == CursorKind.VarDecl && c.Location.File.Name == File)
                            {
                                _Parser.CParser_VisitVariableCursor(c);
                            }
                        }
                    }

                }

                _CurrentTu.Dispose();//Huge object dispose after translating to internal object of smaller size
            }
        }
        /// <summary>
        /// Function to set the current Project model
        /// </summary>
        /// <param name="projectModel"></param>
        public void Clang_SetProjectModel( ProjectModel projectModel)
        {
            _Model = projectModel;
            
        }

        public void Dispose()
        {
            _Index.Dispose();
        }
    }
}
