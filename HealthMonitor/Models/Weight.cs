using SQLite;

namespace HealthMonitor.Models
{
    [Table("Weight")]
    public class Weight
    {
        [Column("WeightID")]
        [PrimaryKey, Unique, AutoIncrement]
        public int WeightID { get; set; }

        [Column("WeightTime")]
        public string WeightTime { get; set; }

        [Column("WeightAmount")]
        public decimal WeightAmount { get; set; }

        [Column("BMI")]
        public decimal BMI { get; set; }

        [Column("BodyFat")]
        public decimal BodyFat { get; set; }

        [Column("BodyWater")]
        public decimal BodyWater { get; set; }

        [Column("MuscleMass")]
        public decimal MuscleMass { get; set; }

        [Column("CalMax")]
        public decimal CalMax { get; set; }

        [Column("WeightNotes")]
        public string WeightNotes { get; set; }

        [Column("DailyLogId")]
        public int DailyLogId { get; set; }
    }
}
