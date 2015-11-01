using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models.EventArguments
{
    public class CpuUpdateEventArgs : EventArgs
    {
        public CpuCollection Data { set; get; }

        public CpuUpdateEventArgs(CpuCollection collection)
        {
            Data = collection;
        }
    }
}
