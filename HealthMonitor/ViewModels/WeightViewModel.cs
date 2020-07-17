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
    public class WeightViewModel : ViewModelBase
    {
        public ICommand BtnSetWeightTime { get; set; }
        public ICommand BtnAddWeight { get; set; }
        public ICommand BtnCancelWeight { get; set; }
        public ICommand BtnEditReading { get; set; }
        public ICommand BtnDeleteReading { get; set; }

        private int _dailyLogId;
        private readonly SQLiteConnection _conn;

        private int _weightId;
        public int WeightId
        {
            get => _weightId;
            set
            {
                if (_weightId == value) return;
                _weightId = value;
                OnPropertyChanged("WeightId");
            }
        }

        private string _weightTime;
        public string WeightTime
        {
            get => _weightTime;
            set
            {
                if (_weightTime == value) return;
                _weightTime = value;
                OnPropertyChanged("WeightTime");
            }
        }

        private decimal _weightAmount;
        public decimal WeightAmount
        {
            get => _weightAmount;
            set
            {
                if (_weightAmount == value) return;
                _weightAmount = value;
                OnPropertyChanged("WeightAmount");
            }
        }

        private decimal _wtBmi;
        public decimal WtBmi
        {
            get => _wtBmi;
            set
            {
                if (_wtBmi == value) return;
                _wtBmi = value;
                OnPropertyChanged("WtBmi");
            }
        }

        private decimal _wtBodyFat;
        public decimal WtBodyFat
        {
            get => _wtBodyFat;
            set
            {
                if (_wtBodyFat == value) return;
                _wtBodyFat = value;
                OnPropertyChanged("WtBodyFat");
            }
        }

        private decimal _wtBodyWater;
        public decimal WtBodyWater
        {
            get => _wtBodyWater;
            set
            {
                if (_wtBodyWater == value) return;
                _wtBodyWater = value;
                OnPropertyChanged("WtBodyWater");
            }
        }

        private decimal _wtMuscleMass;
        public decimal WtMuscleMass
        {
            get => _wtMuscleMass;
            set
            {
                if (_wtMuscleMass == value) return;
                _wtMuscleMass = value;
                OnPropertyChanged("WtMuscleMass");
            }
        }

        private decimal _wtCalMax;
        public decimal WtCalMax
        {
            get => _wtCalMax;
            set
            {
                if (_wtCalMax == value) return;
                _wtCalMax = value;
                OnPropertyChanged("WtCalMax");
            }
        }

        private string _wtNotes;
        public string WtNotes
        {
            get => _wtNotes;
            set
            {
                if (_wtNotes == value) return;
                _wtNotes = value;
                OnPropertyChanged("WtNotes");
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        private Weight _selectedWeightReading;
        public Weight SelectedWeightReading
        {
            get => _selectedWeightReading;
            set
            {
                if (_selectedWeightReading == value) return;
                _selectedWeightReading = value;
                OnPropertyChanged("SelectedWeightReading");
            }
        }

        private ObservableCollection<Weight> _weights;
        public ObservableCollection<Weight> Weights
        {
            get => _weights;
            set
            {
                if (_weights == value) return;
                _weights = value;
                OnPropertyChanged("Weights");
            }
        }

        private int indexId = -1;
        public WeightViewModel(int dailylogId)
        {
            _dailyLogId = dailylogId;
            _conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            
            BtnSetWeightTime = new RelayCommand(SetWeightTime);
            BtnAddWeight = new RelayCommand(SubmitReading);
            BtnCancelWeight = new RelayCommand(CancelWeightReading);
            BtnEditReading = new RelayCommand(EditWeightReading);
            BtnDeleteReading = new RelayCommand(DeleteWeightReading);
            StatusMessage = "Loading...";

            BuildWeightList();
            WeightTime = string.Format("{0:t}", DateTime.Now);
            StatusMessage = "";
        }

        private void DeleteWeightReading(object obj)
        {
            throw new NotImplementedException();
        }

        private void EditWeightReading(object obj)
        {
            throw new NotImplementedException();
        }

        private void CancelWeightReading(object obj)
        {
            throw new NotImplementedException();
        }

        private void SubmitReading(object obj)
        {
            try
            {
                StatusMessage = "Saving...";
                Weight dbWt = Weights.FirstOrDefault(x => x.WeightID == WeightId || x.WeightID == indexId);
                if (dbWt == null)
                {
                    Weight wt = new Weight();
                    wt.WeightID = indexId;
                    wt.WeightTime = WeightTime;
                    wt.WeightAmount = WeightAmount;
                    wt.BMI = WtBmi;
                    wt.BodyFat = WtBodyFat;
                    wt.BodyWater = WtBodyWater;
                    wt.MuscleMass = WtMuscleMass;
                    wt.CalMax = WtCalMax;
                    wt.WeightNotes = WtNotes;
                    wt.DailyLogId = _dailyLogId;
                    Weights.Add(wt);
                    ClearTextboxes();
                    indexId--;
                    StatusMessage = "";
                    return;
                }

                dbWt.WeightTime = WeightTime;
                dbWt.WeightAmount = WeightAmount;
                dbWt.BMI = WtBmi;
                dbWt.BodyFat = WtBodyFat;
                dbWt.BodyWater = WtBodyWater;
                dbWt.MuscleMass = WtMuscleMass;
                dbWt.CalMax = WtCalMax;
                dbWt.WeightNotes = WtNotes;
                CollectionViewSource.GetDefaultView(Weights).Refresh();
                ClearTextboxes();
                StatusMessage = "";
                return;
            }
            catch (Exception)
            {
                StatusMessage = "Error saving the weight reading";
            }
        }

        private void SetWeightTime(object obj)
        {
            WeightTime = string.Format("{0:t}", DateTime.Now);
        }

        private void BuildWeightList()
        {
            if (_dailyLogId > 0)
            {
                IList<Weight> wt = _conn.Table<Weight>().Where(x => x.DailyLogId == _dailyLogId).ToList();
                Weights = new ObservableCollection<Weight>(wt);
                return;
            }
            Weights = new ObservableCollection<Weight>();
            return;
        }

        public void ClearTextboxes()
        {
            WeightId = 0;
            WeightTime = string.Format("{0:t}", DateTime.Now);
            WeightAmount = 0;
            WtBmi = 0;
            WtBodyFat = 0;
            WtBodyWater = 0;
            WtMuscleMass = 0;
            WtCalMax = 0;
            WtNotes = string.Empty;
        }
    }
}
