using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CC.Logic;
using CC.Metrics;
using Newtonsoft.Json;

namespace ComputerControl.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner();
            runner.Start();

            while (true)
            {
                var key = System.Console.ReadKey();

                if (key.Key == ConsoleKey.Q)
                    break;
            }

            runner.Stop();
        }
    }
}
