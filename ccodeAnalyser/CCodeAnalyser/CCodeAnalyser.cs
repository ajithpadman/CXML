using CCodeFramework.Interfaces;
using System;
using System.Reflection;
using CCodeTypes.Types;
using CCodeTypes.Xml;
using CCodeAnalyser.Factory;

namespace CCodeAnalyser
{

    /// <summary>
    /// Main Class for analysing the C Source Tree
    /// </summary>
    public class CCodeAnalyser
    {
        IFactory _factory = null;
        ProjectModel _ProjectModel = null;
        public CCodeAnalyser()
        {
            _factory = new ClangFactory();
          
        }
        /// <summary>
        /// Get the Factory Class Object for usage in other Components
        /// </summary>
        /// <returns></returns>
        public IFactory GetFactory()
        {
            return _factory;
        }
        /// <summary>
        /// Get the Project model used for other components
        /// </summary>
        /// <returns></returns>
        public ProjectModel GetProjectModel()
        {
            return _ProjectModel;
        }
        /// <summary>
        /// Main Entry point for the C Source Tree Analysis.
        /// Analyses the C files and .h Files
        /// </summary>
        /// <param name="args"> Contain the Arguments required for the Clang </param>
        /// <returns></returns>
        public CXml Analyse(string[] args)
        {
            //with this call the Args is converted in to ProjectModel object by the Factory
            _ProjectModel = _factory.Fac_getProjectModel(args);
            if(_ProjectModel!= null)
            {
                //Construct and return the Clanginterface object
                IClang clang = _factory.Fac_getClangInterface();
                //set the project model to be used in the Clanginterface
                clang.Clang_SetProjectModel(_ProjectModel);
                Console.WriteLine("\n**********************************CCodeAnalyser**************************\n");

                Console.WriteLine("**********************************Version" + Assembly.GetExecutingAssembly().GetName().Version + "************************");

                Console.WriteLine("\n************************************************************************\n");

                Console.WriteLine("\n*****************Parsing Source Files*******************\n");
                //Iterate through all the Source files in the project model and generate the Translation Unit
                foreach (string src in _ProjectModel.SourceFiles)
                {
                    Console.WriteLine(src);
                    //find all the functions inside the source file
                    clang.Clang_FindFunctions(src);
                    //find all the global variables in the Source file
                    clang.Clang_FindGlobalVariable(src);
                }


                Console.WriteLine("\n*****************Parsing Header Files*******************\n");
                //Parse all the header files
                foreach (string header in _ProjectModel.HeaderFiles)
                {
                    Console.WriteLine(header);
                    //find all the functions in the header files
                    //no need to analyse for the global variables as they are only found in the Source code only
                    clang.Clang_FindFunctions(header);

                }
                //Serialize the Collected C Elements in to Cxml format 
                return _factory.Fac_getDataBase().PublishDataBase(_ProjectModel.CodeXmlModel);
            }
            else
            {
                Console.WriteLine("Invalid Arguments");
                return null;
            }
            



        }
    }
}
