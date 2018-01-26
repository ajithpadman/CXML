using System;
using System.Collections.Generic;
using System.Linq;
using CCodeFramework.Interfaces;
using CCodeTypes.Xml;
using System.IO;

using System.Xml.Serialization;
using CCodeTypes.Types;
using CCodeAnalyser.DBInterface;
using CCodeAnalyser.ClangInterface;

namespace CCodeAnalyser.Factory
{
    public class ClangFactory : IFactory
    {
        ProjectModel model = null;
        IAstDataBase _DB = null;
        ICParser _parser = null;
        ICTypeParser _TypeParser = null;
        
        ClangInterface.ClangInterface _Clang = null;
        Dictionary<string, string> _SourceGroups = null;
        Dictionary<string, SWCType> _SWCType = null;

        public ClangFactory()
        {
            _DB = new AstDataBase(this);
            _TypeParser = new CTypeParser(this);
            _parser = new CParser(this);
            _SourceGroups = new Dictionary<string, string>();
            _SWCType = new Dictionary<string, SWCType>();
            _Clang = new ClangInterface.ClangInterface(this);
        }
        public IAstDataBase Fac_getDataBase()
        {
            return _DB;
        }
        public ICTypeParser fac_getTypeParser()
        {
            return _TypeParser;
        }
        public ICParser fac_getCParser()
        {
            return _parser;
        }
        public IClang Fac_getClangInterface()
        {
            return _Clang;
        }
        public Dictionary<string, string> Fac_GetSWCConfig()
        {
            return _SourceGroups;
        }
        public Dictionary<string, SWCType> Fac_GetSWCType()
        {
            return _SWCType;
        }
        public ProjectModel Fac_getProjectModel(string[] args)
        {
            model = new ProjectModel();
            CommandLineparser parser = new CommandLineparser();
            parser.Init();
            parser.parseOptions(args);
            if (args.Length == 0)
            {

                return null;
            }
            else if (args.Length > 0)
            {

                if (parser.CommandLineOptions["-file"].Count > 0)
                {
                    foreach (string f in parser.CommandLineOptions["-file"])
                    {
                        parsefile(f);
                    }
                }
                else
                {
                    updateModel(parser);
                }

                foreach (string path in model.IncludePaths)
                {
                    path.Replace('/', '\\');
                }
                foreach (string path in model.CommonHeaders)
                {
                    path.Replace('/', '\\');
                }
                foreach (string path in model.LibraryPaths)
                {
                    path.Replace('/', '\\');
                }

                model.IncludePaths = model.IncludePaths.Distinct<string>().ToList();
                model.CommonHeaders = model.CommonHeaders.Distinct<string>().ToList();
                foreach (string path in model.IncludePaths)
                {
                    if (Directory.Exists(path))
                    {
                        model.HeaderFiles.AddRange(Directory.GetFiles(path, "*.h", SearchOption.TopDirectoryOnly));
                    }
                }
                foreach (string path in model.SourcePaths)
                {
                    if (File.Exists(path))
                    {
                        model.SourceFiles.Add(path);
                    }
                }
                model.SourceFiles = model.SourceFiles.Distinct().ToList();
                if (model.IncludePaths.Count == 0)
                {
                    model.HeaderFiles.AddRange(Directory.GetFiles(model.RootDir.Trim(), "*.h", SearchOption.AllDirectories));
                    foreach (string file in model.HeaderFiles)
                    {
                        DirectoryInfo di = new DirectoryInfo(file);
                        while(di.FullName!= model.RootDir)
                        {
                            di = di.Parent;
                            model.IncludePaths.Add(di.FullName);
                        }

                       
                    }
                    model.IncludePaths = model.IncludePaths.Distinct().ToList();

                }
               
                if (model.SourcePaths.Count == 0)
                {
                    model.SourceFiles.AddRange(Directory.GetFiles(model.RootDir.Trim(), "*.c", SearchOption.AllDirectories));
                    foreach (string file in model.SourceFiles)
                    {
                        DirectoryInfo di = new DirectoryInfo(file);

                        while (di.FullName != model.RootDir)
                        {
                            di = di.Parent;
                            model.IncludePaths.Add(di.FullName);
                        }
                    }
                    model.IncludePaths = model.IncludePaths.Distinct().ToList();
                }
                model.HeaderFiles = model.HeaderFiles.Distinct().ToList();
                model.SourceFiles = model.SourceFiles.Distinct().ToList();
            }
            Updateclangoptions();
            updateSwcConfig();

            return model;


        }
        private void updateSwcConfig()
        {
            if (File.Exists(model.SWCConfig))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SWCConfig));
                    using (var fs = new FileStream(model.SWCConfig, FileMode.OpenOrCreate))
                    {
                        SWCConfig config = (SWCConfig)serializer.Deserialize(fs);
                        foreach (SWC s in config.SWC)
                        {
                            if(s.Parent!= null)
                            {
                                if (_SourceGroups.ContainsKey(s.Parent) == false)
                                {
                                    _SourceGroups.Add(s.Parent, s.Name);
                                }
                                if (_SWCType.ContainsKey(s.Name) == false)
                                {
                                    _SWCType.Add(s.Name, s.Type);

                                }
                            }
                          

                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }
        }
        private void parsefile(string file)
        {
            if (File.Exists(file))
            {
                StreamReader reader = new StreamReader(file);
                string content = reader.ReadToEnd();
                string[] elements = content.Split(' ');
                CommandLineparser p = new CommandLineparser();
                p.Init();
                p.parseOptions(elements);
                updateModel(p);


            }

        }
        private void updateModel(CommandLineparser parser)
        {
            if (parser.CommandLineOptions["-root"].Count > 0)
            {
                model.RootDir = parser.CommandLineOptions["-root"][parser.CommandLineOptions["-root"].Count - 1];
            }
            if (parser.CommandLineOptions["-include"].Count > 0)
            {
                model.CommonHeaders.AddRange(parser.CommandLineOptions["-include"]);
            }
            if (parser.CommandLineOptions["-I"].Count > 0)
            {
                model.IncludePaths.AddRange(parser.CommandLineOptions["-I"]);
            }
            if (parser.CommandLineOptions["-S"].Count > 0)
            {
                model.SourcePaths.AddRange(parser.CommandLineOptions["-S"]);
            }
            if (parser.CommandLineOptions["-l"].Count > 0)
            {
                model.LibraryPaths.AddRange(parser.CommandLineOptions["-l"]);
            }
            if (parser.CommandLineOptions["-L"].Count > 0)
            {
                model.Libraries.AddRange(parser.CommandLineOptions["-L"]);
            }
            if (parser.CommandLineOptions["-D"].Count > 0)
            {
                model.MacroDefines.AddRange(parser.CommandLineOptions["-D"]);
            }
            if (parser.CommandLineOptions["-O"].Count > 0)
            {
                model.CodeXmlModel = parser.CommandLineOptions["-O"][parser.CommandLineOptions["-O"].Count - 1];
            }
            if (parser.CommandLineOptions["-SWCConfig"].Count > 0)
            {
                model.SWCConfig = parser.CommandLineOptions["-SWCConfig"][parser.CommandLineOptions["-SWCConfig"].Count - 1];
            }
            if(parser.CommandLineOptions["-g"].Count > 0)
            {
                model.UmlModel = parser.CommandLineOptions["-g"][parser.CommandLineOptions["-g"].Count - 1];
            }
            if (parser.CommandLineOptions["-F"].Count > 0)
            {
                model.SrcListFile = parser.CommandLineOptions["-F"][parser.CommandLineOptions["-F"].Count - 1];
                if(File.Exists(model.SrcListFile))
                {
                    model.SourcePaths.Clear();
                    string line = "";
                    using (StreamReader reader = new StreamReader(model.SrcListFile))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if(File.Exists(line))
                            {
                                model.SourcePaths.Add(line);
                            }
                        }

                    }
                }
                
            }
        }
        protected void Updateclangoptions()
        {

            if (model != null)
            {
                model.ClangOptions.Clear();
                model.ClangOptions.Add("-fno-ms-compatibility");
              

                foreach (string str in model.IncludePaths)
                {
                    model.ClangOptions.Add("-I");
                    model.ClangOptions.Add(str);
                }

                foreach (string str in model.CommonHeaders)
                {

                    model.ClangOptions.Add("-include");
                    model.ClangOptions.Add(str);


                }

                foreach (string macro in model.MacroDefines)
                {
                    model.ClangOptions.Add("-D");
                    model.ClangOptions.Add(macro);
                }
                model.ClangOptions.Add("-Wall");
                model.ClangOptions.Add("-MMD");
                model.ClangOptions.Add("-MP");

            }



        }

        public Function Fac_GetXmlFunctionType(CFunction f)
        {
            Function ft = new Function();
            ft.Name = f.Name;
            ft.ID = f.ID;
            ft.File = f.File;
            ft.Column = f.Column.ToString();
            ft.Line = f.Line.ToString();
            ft.StorageClass = f.StorageClass.ToString();
            ft.IsDefinition = f.IsDefinition;
            ft.ReturnIDRef = f.ReturnType;
            foreach (CVariable v in f.Parameters)
            {
                ft.Parameters.Add(Fac_GetXmlVariableType(v));
            }
            return ft;


        }

        public Variable Fac_GetXmlVariableType(CVariable v)
        {
            Variable vt = new Variable();
            vt.Name = v.Name;
            vt.ID = v.ID;
            vt.File = v.File;
            vt.Line = v.Line.ToString();
            vt.TypeIDref = v.Type;
            return vt;
        }

        public DataType Fac_GetXmlDataType(CDataType d)
        {
            DataType dt = new DataType();
            dt.Name = d.Name;
            dt.Kind = d.Kind.ToString();
            dt.File = d.File;
            dt.ID = d.ID;
            dt.Column = d.Column.ToString();
            dt.Line = d.Line.ToString();
            dt.pointToIDref = d.Parent;
            dt.underlyingTypeIDref = d.Parent;
            dt.Qualifier = d.Qualifier.ToString();
            dt.StorageClass = d.StorageClass.ToString();

            foreach (CVariable v in d.Children)
            {
                dt.Attributes.Add(Fac_GetXmlVariableType(v));

            }
            return dt;
        }


        public string Fac_GetComponentName(CElement function)
        {

            string dir = (function.File);
            string path = Path.GetFileName(dir);
            string[] keys = _SourceGroups.Keys.Where(x => dir.Contains(x)).ToArray();
            if (keys.Count() > 0)
            {
                path = _SourceGroups[keys[keys.Count() - 1]];
            }
            return path;


        }
        public string Fac_GetInterfaceName(CFunction function)
        {
            string name = Path.GetFileNameWithoutExtension(function.File);
            if (function.IsDefinition)
            {
                name = name + "_impl";
            }
            return name;

        }

      
    }
}
