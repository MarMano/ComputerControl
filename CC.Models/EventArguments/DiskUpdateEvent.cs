using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models.EventArguments
{
    public class DiskUpdateEvent : EventArgs
    {
        public IList<SingleDisk> Data { set; get; }

        public DiskUpdateEvent(IList<SingleDisk> list)
        {
            Data = list;
        }
    }
}
