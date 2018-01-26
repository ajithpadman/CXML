using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCodeAnalyser.Factory
{
    public class CommandLineparser
    {
        Dictionary<string, List<string>> _CommandLineOptions = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> CommandLineOptions
        {
            get { return _CommandLineOptions; }
            set { _CommandLineOptions = value; }
        }
        public virtual void Init()
        {
            CommandLineOptions.Add("-root", new List<string>());
            CommandLineOptions.Add("-SWCConfig", new List<string>());
            CommandLineOptions.Add("-include", new List<string>());
            CommandLineOptions.Add("-I", new List<string>());
            CommandLineOptions.Add("-S", new List<string>());
            CommandLineOptions.Add("-D", new List<string>());
            CommandLineOptions.Add("-l", new List<string>());
            CommandLineOptions.Add("-L", new List<string>());
            CommandLineOptions.Add("-O", new List<string>());
            CommandLineOptions.Add("-g", new List<string>());
            CommandLineOptions.Add("-file", new List<string>());
            CommandLineOptions.Add("-F", new List<string>());

        }
        public void parseOptions(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                foreach (string sep in CommandLineOptions.Keys)
                {
                    if (args[i].StartsWith(sep))
                    {
                        string[] seperator = new string[1];
                        seperator[0] = sep;
                        string[] arr = args[i].Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                        if (arr.Length == 0)
                        {
                            CommandLineOptions[sep].Add(args[i + 1].Trim());
                            i++;
                        }
                        else
                        {
                            CommandLineOptions[sep].Add(arr[0].Trim());

                        }

                    }
                }
            }
        }
    }
}
