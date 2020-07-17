using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Models
{
    public class DailyLogsModel
    {
        public DailyLog Logs { get; set; }
        public List<Weight> Weights { get; set; }
        public List<BloodPressure> BloodPressures { get; set; }
    }
}
