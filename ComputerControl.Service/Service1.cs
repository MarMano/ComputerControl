using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CC.Logic;

namespace ComputerControl.Service
{
    public partial class Service1 : ServiceBase
    {
        private readonly Runner _runner;

        public Service1()
        {
            ServiceName = "ComputerControl";
            InitializeComponent();
            _runner = new Runner();
        }

        protected override void OnStart(string[] args)
        {
            _runner.Start();
        }

        protected override void OnStop()
        {
            _runner.Stop();
        }
    }
}
