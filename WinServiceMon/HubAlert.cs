using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServiceMon
{
    public class HubAlert
    {
        public HubAlert()
        {
            TimeStamp = DateTime.Now;
        }
        public string ServiceName { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get;  set; }
    }
}
