using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persenter_server
{
    public class Help
    {

        const int TAB_WIDTH = 8;

        public static void ShowHelp(string val)
        {
            string help = Properties.Resources.ResourceManager.GetString(val);
            if (help == null)
                throw new Exception(string.Format("No help for {0}", val));
            else
                try
                {
                    AdvancedConsole.Write(GetConsoleFormat(help));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("No help for {0}", val));
                }
        }

        public static string GetConsoleFormat(string json)
        {

                CommandHelp cHelp = JsonConvert.DeserializeObject<CommandHelp>(json);
                string output = "";

                if (cHelp != null)
                {
                    output += cHelp.ToString();
                }

                return output;



        }


        /// <summary>
        /// Store command help data
        /// </summary>
        public class CommandHelp
        {
            public string title;
            public string description;
            public Param[] parameters;

            /// <summary>
            /// Get the command help as string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string output = "";

                if (!string.IsNullOrEmpty(title))
                    output += string.Format("NAME\n\t{0}\n\n", title);

                if (!string.IsNullOrEmpty(description))
                    output += string.Format("DESCRIPTION\n{0}\n\n", description.AutoReturn(-1, "\t"));

                // If at least on param, get params details
                if (parameters != null && parameters.Length > 0)
                {
                    output += "PARAMETERS\n";
                    foreach(Param param in parameters)
                    {
                        output += string.Format("\t{0}\n", param.ToString());
                    }
                }
                
                return output;
            }

            /// <summary>
            /// Store a single command param
            /// </summary>
            public class Param
            {
                public string type;
                public string name;
                public string description;

                /// <summary>
                /// Get param info as string
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    string output = "";

                    if(!string.IsNullOrEmpty(name))
                    {
                        if (!string.IsNullOrEmpty(type))
                            output += string.Format("{0}:{1}", name, type);
                        else
                            output += name;

                        if(!string.IsNullOrEmpty(description))
                            output += string.Format("\n\n\t\x1B[4mDESCRIPTION\x1B[0m\n{0}\n\n", description.AutoReturn(Console.WindowWidth - 24, "\t\t"));
                    }

                    return output;
                }
            } 
        }
    }
}
