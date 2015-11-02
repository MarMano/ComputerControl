using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Logic.Commands
{
    public abstract class BaseCommand
    {
        public abstract string Handle(IDictionary<string, string> arguments);
    }
}
