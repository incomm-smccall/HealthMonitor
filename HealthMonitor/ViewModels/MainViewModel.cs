using HealthMonitor.Config;
using HealthMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        private MainLog _selectedHealthItem;
        public MainLog SelectedHealthItem
        {
            get { return _selectedHealthItem; }
            set
            {
                if (_selectedHealthItem == value) return;
                _selectedHealthItem = value;
                ResetAllSelectedLogs();
                SetDailyLogSelected(_selectedHealthItem.DailyLogId);
                OnPropertyChanged("SelectedHealthItem");
            }
        }

        private ObservableCollection<MainLog> _mainLogs;
        public ObservableCollection<MainLog> MainLogs
        {
            get { return _mainLogs; }
            set
            {
                if (_mainLogs == value) return;
                _mainLogs = value;
                OnPropertyChanged("MainLogs");
            }
        }

        public MainViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            _workspaces = workspaces;
            ResetAllSelectedLogs();
            BuildMainLogs();
        }

        private void BuildMainLogs()
        {
            MainLogs = new ObservableCollection<MainLog>();
            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);

            IEnumerable<DailyLog> logs = conn.Table<DailyLog>().Reverse();
            foreach (DailyLog log in logs)
            {
                MainLog mainlog = new MainLog();
                mainlog.DailyLogId = log.DailyLogID;
                mainlog.LogDateTime = log.LogDateTime;
                BloodPressure bp = GetLastBpReading(log.DailyLogID);
                if (bp != null)
                {
                    mainlog.Bp = $"{bp.BPSystolic} / {bp.BPDiastolic}";
                    mainlog.Pulse = bp.BPPulseRate.ToString();
                }
                Weight wt = GetLastWeightReading(log.DailyLogID);
                mainlog.WeightAmt = wt.WeightAmount.ToString();
                MainLogs.Add(mainlog);
            }
        }

        private void ResetAllSelectedLogs()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false))
            {
                IEnumerable<DailyLog> selLogs = conn.Table<DailyLog>().Where(x => x.DailyLogIsSelected == true);
                if (selLogs.Any())
                {
                    foreach (DailyLog dl in selLogs)
                    {
                        dl.DailyLogIsSelected = false;
                        conn.Update(dl);
                    }
                }
            }
        }

        private void SetDailyLogSelected(int dailylogId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false))
            {
                DailyLog dl = conn.Table<DailyLog>().FirstOrDefault(x => x.DailyLogID == dailylogId);
                dl.DailyLogIsSelected = true;
                conn.Update(dl);
            }
        }

        private BloodPressure GetLastBpReading(int dailyId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite))
            {
                IEnumerable<BloodPressure> bpReadings = conn.Table<BloodPressure>().Where(x => x.DailyLogID == dailyId).Reverse();
                if (bpReadings.Any())
                {
                    return bpReadings.First();
                }
                return new BloodPressure();
            }
        }

        private Weight GetLastWeightReading(int dailyId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite))
            {
                IEnumerable<Weight> wtReadings = conn.Table<Weight>().Where(x => x.DailyLogId == dailyId).Reverse();
                if (wtReadings.Any())
                {
                    return wtReadings.First();
                }
                return new Weight();
            }
        }
    }
}
