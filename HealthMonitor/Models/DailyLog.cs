using SQLite;
using System;

namespace HealthMonitor.Models
{
    [Table("DailyLog")]
    public class DailyLog
    {
        [Column("DailyLogID")]
        [PrimaryKey, AutoIncrement, Unique]
        public int DailyLogID { get; set; }

        [Column("LogDateTime")]
        public string LogDateTime { get; set; }

        [Column("DailyLogNotes")]
        public string DailyLogNotes { get; set; }

        [Column("DailyLogIsSelected")]
        public bool DailyLogIsSelected { get; set; }
    }
}
