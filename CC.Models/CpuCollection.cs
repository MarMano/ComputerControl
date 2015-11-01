using System.Collections.Generic;

namespace CC.Models
{
    public class CpuCollection
    {
        public IList<SingleCpu> SingleCpu { set; get; }
        public SingleCpu Total { set; get; }

        public CpuCollection()
        {
            SingleCpu = new List<SingleCpu>();
        }
    }
}
