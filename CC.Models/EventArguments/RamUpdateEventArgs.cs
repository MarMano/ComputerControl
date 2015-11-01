using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models.EventArguments
{
    public class RamUpdateEventArgs : EventArgs
    {
        public long Avaliable { set; get; }
        public long Total { set; get; }

        public RamUpdateEventArgs(long avaliable, long total)
        {
            Avaliable = avaliable;
            Total = total;
        }
    }
}
