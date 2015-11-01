using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CC.Models.EventArguments;

namespace CC.Metrics
{
    public class RamUsage
    {
        private Timer _updateTimer;

        public delegate void UpdateTimerElapsed (object sender, RamUpdateEventArgs args);
        public event UpdateTimerElapsed Update;

        public RamUsage(int interval = 1000)
        {
            _updateTimer = new Timer
            {
                AutoReset = true,
                Interval = interval
            };

            _updateTimer.Elapsed += UpdateTimerOnElapsed;

            _updateTimer.Start();
        }

        public void Stop()
        {
            _updateTimer.Stop();

        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var ramUsed = new PerformanceCounter("Memory", "Available MBytes");
            var ramTotal = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);

            if(Update != null)
                Update(this, new RamUpdateEventArgs(ramUsed.RawValue, (long)ramTotal));
        }
    }
}
