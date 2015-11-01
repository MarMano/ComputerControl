using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ComputerControl.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Trying to install");

                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        callInstallUtil(new[] {Assembly.GetExecutingAssembly().Location});
                        break;

                    case "--uninstall":
                        callInstallUtil(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }

                Console.WriteLine("Installed");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        public static string InstallUtilPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
        private static bool callInstallUtil(string[] installUtilArguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(InstallUtilPath, "installutil.exe");
            proc.StartInfo.Arguments = String.Join(" ", installUtilArguments);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            string outputResult = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //  ---check result---
            if (proc.ExitCode != 0)
            {
                //Errors.Add(String.Format("InstallUtil error -- code {0}", proc.ExitCode));
                return false;
            }

            return true;
        } 
    }
}
