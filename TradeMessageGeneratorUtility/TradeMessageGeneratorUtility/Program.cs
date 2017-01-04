using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TradeMessageGenerator
{
    static class Program
    {
        //To redirect messages to commadline window
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Invoked by a automation script
            if (args != null && args.Length > 0)
            {
                if (args.Length==1 && args[0].ToLower() == "/generate")
                {
                    if (Directory.Exists(AppSettings.DirectoryName))
                    {
                        TradeMessage tMsg = new TradeMessage();
                        tMsg.GenerateCombination();
                        AttachConsole(-1);
                        Console.WriteLine("Successfully generated sample trade messages.");
                        Console.WriteLine();
                    }

                }
                else if (args.Length == 2 && args[0].ToLower() == "/compare")
                {
                    string directoryToMonitor = args[1];
                    if (Directory.Exists(directoryToMonitor))
                    {
                        TradeMessage tMsg = new TradeMessage();
                        tMsg.CompareLogMessages(directoryToMonitor);
                        AttachConsole(-1);
                        Console.WriteLine("Successfully compared all trade messages.");
                        Console.WriteLine();
                    }

                }
                else
                {
                    AttachConsole(-1);
                    Console.WriteLine("Invalid Commandline Arguments.");
                    Console.WriteLine("Please use below parameters to process further,");
                    Console.WriteLine("1. /generate : To generate sample data");
                    Console.WriteLine("2. /compare folderpath : To compare log messages");
                    Console.WriteLine();
                }

            }
            else
            {
                //User wants to generate trades through UI
                if (Directory.Exists(AppSettings.DirectoryToMonitor))
                {
                    Application.Run(new CompareWindow());
                }
            }
        }

    }
}
