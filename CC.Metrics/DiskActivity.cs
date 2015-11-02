using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CC.Models;
using CC.Models.EventArguments;

namespace CC.Metrics
{
    public class DiskActivity
    {
        private Timer _updateTimer;

        public delegate void UpdateTimerElapsed(object sender, DiskUpdateEvent args);
        public event UpdateTimerElapsed Update;

        private readonly PerformanceCounter _performanceCounter;
        private readonly string[] _instances;
        private readonly Dictionary<string, CounterSample> _counterSamples;

        public DiskActivity(int interval = 1000)
        {
            _performanceCounter = new PerformanceCounter("PhysicalDisk", "% Idle Time");
            var performanceCounterCategory = new PerformanceCounterCategory("PhysicalDisk");
            _instances = performanceCounterCategory.GetInstanceNames();
            _counterSamples = new Dictionary<string, CounterSample>();

            foreach (var s in _instances)
            {
                _performanceCounter.InstanceName = s;
                _counterSamples.Add(s, _performanceCounter.NextSample());
            }

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

        public int Count()
        {
            return DriveInfo.GetDrives().Count(x => x.DriveType == DriveType.Fixed);
        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var driveInfo = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).ToList();

            var collection = new List<SingleDisk>();

            foreach (var s in _instances)
            {
                _performanceCounter.InstanceName = s;

                var calculation = Calculate(_counterSamples[s], _performanceCounter.NextSample());
                var load = calculation > 0 ? (int)calculation : 0;

                if (!s.Contains("Total"))
                {
                    var id = int.Parse(s.Substring(0, 1));

                    collection.Add(new SingleDisk()
                    {
                        Id = id,
                        Letter = driveInfo[id].Name.Substring(0, 1),
                        Usage = load
                    });
                }
                _counterSamples[s] = _performanceCounter.NextSample();
            }

            if (Update != null)
                Update(this, new DiskUpdateEvent(collection));
        }

        private static double Calculate(CounterSample oldSample, CounterSample newSample)
        {
            double difference = newSample.RawValue - oldSample.RawValue;
            double timeInterval = newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec;
            if (timeInterval != 0)
                return 100 * (1 - (difference / timeInterval));

            return 0;
        }
    }
}
