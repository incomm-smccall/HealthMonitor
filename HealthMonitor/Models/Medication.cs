using SQLite;

namespace HealthMonitor.Models
{
    [Table("Medication")]
    public class Medication
    {
        [Column("MedicationID")]
        [PrimaryKey, Unique, AutoIncrement]
        public int MedicationID { get; set; }

        [Column("MedicationTime")]
        public string MedicationTime { get; set; }

        [Column("MedicationName")]
        public string MedicationName { get; set; }

        [Column("MedicationDose")]
        public string MedicationDose { get; set; }

        [Column("RememberMed")]
        public bool RememberMed { get; set; }

        [Column("DailyLogID")]
        public int DailyLogID { get; set; }
    }
}
