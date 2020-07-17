using SQLite;

namespace HealthMonitor.Models
{
    [Table("Medicine")]
    public class Medicine
    {
        [Column("MedicineID")]
        [PrimaryKey, Unique, AutoIncrement]
        public int MedicineID { get; set; }

        [Column("MedicineName")]
        public string MedicineName { get; set; }

        [Column("MedicineDose")]
        public string MedicineDose { get; set; }
    }
}
