using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using CC.Models;
using CC.Models.EventArguments;

namespace CC.Metrics
{
    public class CpuLoad
    {
        private Timer _updateTimer;

        public delegate void UpdateTimerElapsed (object sender, CpuUpdateEventArgs args);
        public event UpdateTimerElapsed Update;

        private readonly PerformanceCounter _performanceCounter;
        private readonly string[] _instances;
        private readonly Dictionary<string, CounterSample> _counterSamples;

        public CpuLoad(int interval = 1000)
        {
            _performanceCounter = new PerformanceCounter("Processor Information", "% Processor Time");
            var performanceCounterCategory = new PerformanceCounterCategory("Processor Information");
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

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var collection = new CpuCollection();

            foreach (var s in _instances)
            {
                _performanceCounter.InstanceName = s;

                var calculation = Calculate(_counterSamples[s], _performanceCounter.NextSample());
                var load = calculation > 0 ? (int)calculation : 0;

                if (s.Contains("Total"))
                {
                    collection.Total = new SingleCpu()
                    {
                        Id = 0,
                        Load = load
                    };
                }
                    
                else
                {
                    var cpuId = int.Parse(s.Substring(s.Length - 1, 1));
                    var newCpu = new SingleCpu
                    {
                        Load = load,
                        Id = cpuId
                    };

                    collection.SingleCpu.Add(newCpu);
                }

                _counterSamples[s] = _performanceCounter.NextSample();
            }

            if (Update != null)
                Update(this, new CpuUpdateEventArgs(collection));
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
