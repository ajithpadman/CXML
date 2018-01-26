using CCodeTypes.Types;
using CCodeTypes.Xml;
using System;
using System.Linq;
using CCodeAnalyser;
namespace ModelGenerator
{
    class Program
    {
        private static void printhelp()
        {
            Console.WriteLine("Use the tool with below options");
            Console.WriteLine("     -root rootDirectory: Specify the root directory on which the tool will operate");
            Console.WriteLine("     -SWCConfig XML file specifying Source code to SW component Name mapping Refer to Example SWCConfig.xml file");
            Console.WriteLine("     -O example.xml: Specify the Name of Code Description to be generated");
            Console.WriteLine("     -g example.xml:representation of Code in XML format");
            Console.WriteLine("     -include File.h: similar to gcc -include option. Process file as if #include file appeared as the first line of the primary source file");
            Console.WriteLine("     -I C:\\Example\\path: similar to gcc -I option. use only absolute paths. header file search paths .if no -I option is given the tool will analyse all .h files in -root folder");
            Console.WriteLine("     -S C:\\Example\\Source.c:  specific C files to be analysed");
            Console.WriteLine("     -D Macro Definitions to be enabled while analysing the code. similar to gcc -D option");
            Console.WriteLine("     -l C\\Example\\LibraryPath: similar to gcc -l option. library search paths");
            Console.WriteLine("     -L Libraryname: similar to gcc -L option. Library names");
            Console.WriteLine("Press any Key to continue...");
        }
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() == 0)
                {
                    printhelp();
                    Console.ReadKey();
                    return;
                }
                else
                {


                    if (args[0] == "-help")
                    {
                        printhelp();
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        CCodeAnalyser.CCodeAnalyser Analyser = new CCodeAnalyser.CCodeAnalyser();
                        CXml xmi = Analyser.Analyse(args);
                        CxmlIDDecorator decorator = new CxmlIDDecorator(xmi);
                        ProjectModel model = Analyser.GetProjectModel();
                        decorator.Serialise(model.CodeXmlModel);


                    }
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
       


    
    }
}
