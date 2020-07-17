using HealthMonitor.Config;
using HealthMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace HealthMonitor.ViewModels
{
    public class BloodPressureViewModel : ViewModelBase
    {
        public ICommand BtnSetBpTime { get; set; }
        public ICommand BtnAddReading { get; set; }

        private int _dailyLogId;

        private int _bpId;
        public int BpId
        {
            get => _bpId;
            set
            {
                if (_bpId == value) return;
                _bpId = value;
                OnPropertyChanged("BpId");
            }
        }

        private string _bpTime;
        public string BpTime
        {
            get => _bpTime;
            set
            {
                if (_bpTime == value) return;
                _bpTime = value;
                OnPropertyChanged("BpTime");
            }
        }

        private string _bpSystolic;
        public string BpSystolic
        {
            get => _bpSystolic;
            set
            {
                if (_bpSystolic == value) return;
                _bpSystolic = value;
                OnPropertyChanged("BpSystolic");
            }
        }

        private string _bpDiastolic;
        public string BpDiastolic
        {
            get => _bpDiastolic;
            set
            {
                if (_bpDiastolic == value) return;
                _bpDiastolic = value;
                OnPropertyChanged("BpDiastolic");
            }
        }

        private string _bpPulseRate;
        public string BpPulseRate
        {
            get => _bpPulseRate;
            set
            {
                if (_bpPulseRate == value) return;
                _bpPulseRate = value;
                OnPropertyChanged("BpPulseRate");
            }
        }

        private BloodPressure _selectedBloodPressure;
        public BloodPressure SelectedBloodPressure
        {
            get => _selectedBloodPressure;
            set
            {
                if (_selectedBloodPressure == value) return;
                _selectedBloodPressure = value;
                OnPropertyChanged("SelectedBloodPressure");
            }
        }

        private ObservableCollection<BloodPressure> _bloodPressures;
        public ObservableCollection<BloodPressure> BloodPressures
        {
            get => _bloodPressures;
            set
            {
                if (_bloodPressures == value) return;
                _bloodPressures = value;
                OnPropertyChanged("BloodPressures");
            }
        }

        private int indexId = -1;
        private SQLiteConnection _conn;
        public BloodPressureViewModel(int dailyLogId)
        {
            _conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            BloodPressures = new ObservableCollection<BloodPressure>();
            _dailyLogId = dailyLogId;

            BtnSetBpTime = new RelayCommand(SetBpTime);
            BtnAddReading = new RelayCommand(SubmitReading);

            BuildPressureList();
            BpTime = string.Format("{0:t}", DateTime.Now);
        }

        private void SubmitReading(object obj)
        {
            int.TryParse(BpSystolic, out int systolic);
            int.TryParse(BpDiastolic, out int diastolic);
            int.TryParse(BpPulseRate, out int pulse);
            string status = GetPressureStatus(systolic, diastolic);

            BloodPressure bpm = BloodPressures.FirstOrDefault(x => x.BPID == BpId || x.BPID == indexId);
            if (bpm == null)
            {
                BloodPressure bp = new BloodPressure
                {
                    BPID = indexId,
                    BPTime = BpTime,
                    BPSystolic = systolic,
                    BPDiastolic = diastolic,
                    BPPulseRate = pulse,
                    BPStatus = status,
                    DailyLogID = _dailyLogId
                };
                BloodPressures.Add(bp);
                ClearTextboxes();
                indexId--;
                return;
            }

            bpm.BPTime = BpTime;
            bpm.BPSystolic = systolic;
            bpm.BPDiastolic = diastolic;
            bpm.BPPulseRate = pulse;
            bpm.BPStatus = status;
            CollectionViewSource.GetDefaultView(BloodPressures).Refresh();
            ClearTextboxes();
            return;
        }

        private string GetPressureStatus(int systolic, int diastolic)
        {
            if (systolic < 120 && diastolic < 80) return "Normal";
            if ((systolic >= 120 && systolic <= 129) && diastolic < 80) return "Elevated";
            if ((systolic >= 130 && systolic <= 139) || (diastolic >= 80 && diastolic <= 89)) return "Hypertension - Stage 1";
            if (systolic >= 140 || diastolic >= 90) return "Hypertension - Stage 2";
            if (systolic > 180 && diastolic > 120) return "High Crisis";
            return string.Empty;
        }

        private void BuildPressureList()
        {
            if (_dailyLogId > 0)
            {
                IList<BloodPressure> bp = _conn.Table<BloodPressure>().Where(x => x.DailyLogID == _dailyLogId).ToList();
                BloodPressures = new ObservableCollection<BloodPressure>(bp);
                return;
            }
            BloodPressures = new ObservableCollection<BloodPressure>();
            return;
        }

        public void ClearTextboxes()
        {
            BpId = 0;
            BpTime = string.Format("{0:t}", DateTime.Now);
            BpSystolic = string.Empty;
            BpDiastolic = string.Empty;
            BpPulseRate = string.Empty;
        }

        private void SetBpTime(object obj)
        {
            BpTime = string.Format("{0:t}", DateTime.Now);
        }
    }
}
