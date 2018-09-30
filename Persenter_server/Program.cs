using ProxyServer;
using System;
using System.Threading;

namespace Persenter_server
{
    class Program
    {
        static void Main(string[] args)
        {
            AdvancedConsole.WriteAlternativeColor(Properties.Resources.welcome, ConsoleColor.Cyan);

            Type test = 0.GetType();

            Commands cmds = new Commands();
            cmds.Add("close", (param) => {
                Close();
            });

            cmds.Add("test", new Type[] { Ty.INT, Ty.DOUBLE, Ty.STRING }, (param) => {
                Console.WriteLine("{0}, {1}, {2}", param.Parsed[0], param.Parsed[1], param.Parsed[2]);
            });

            cmds.Add("help", Ty.STRING, (param) => {
                Help.ShowHelp((string)param.Parsed[0]);
            });

            cmds.Add("start", (param) => {
                /*Server serv = new Server();
                serv.Start();*/
                ServerConnected serverConnected = new ServerConnected();
            });



            AdvancedConsole.Start();
            AdvancedConsole.NewCMDLine();

            AdvancedConsole.OnCommandInputEvent += (str) => {
                try
                {
                    cmds.Parse(str);
                }
                catch(Exception ex)
                {
                    AdvancedConsole.WriteError(ex.Message);
                }
            };

            AdvancedConsole.OnInputEvent += (str) =>
            {
                AdvancedConsole.NewCMDLine();
            };

            int i = 0;
            while (i++ < 100000) {
                Thread.Sleep(100);
            }

            Close();
        }

        static void Close()
        {
            AdvancedConsole.Stop();
            Logger.Log().Info("Program stoped");
            Environment.Exit(0);
        }
    }
}
