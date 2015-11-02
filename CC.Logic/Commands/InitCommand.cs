using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC.Metrics;
using Newtonsoft.Json;

namespace CC.Logic.Commands
{
    public class InitCommand : BaseCommand
    {
        public override string Handle(IDictionary<string, string> arguments)
        {
            var cpu = new CpuLoad();
            var disk = new DiskActivity();

            var returnString = JsonConvert.SerializeObject(new { Type = "SystemInfo", Data = new { Cpu = cpu.Count(), Disk = disk.Count() } });
            return returnString;
        }
    }
}
