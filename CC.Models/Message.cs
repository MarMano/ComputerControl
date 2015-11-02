using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models
{
    public class Message
    {
        public string Type { set; get; }
        public IDictionary<string, string> Arguments { set; get; } 
    }
}
