using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Models
{
    public class MainLog
    {
        public int DailyLogId { get; set; }
        public string LogDateTime { get; set; }
        public string WeightAmt { get; set; }
        public string Bp { get; set; }
        public string Pulse { get; set; }
    }
}
