using HealthMonitor.Config;
using HealthMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace HealthMonitor.ViewModels
{
    public class EditLogViewModel : ViewModelBase
    {
        public ICommand BtnSetDateTime { get; set; }
        public ICommand BtnSaveLog { get; set; }
        public ICommand BtnCancelLog { get; set; }

        private ObservableCollection<ViewModelBase> _workspaces;
        private SQLiteConnection _conn;
        private int _dailylogId;

        private DailyLog _log;
        public DailyLog Log
        {
            get => _log;
            set
            {
                if (_log == value) return;
                _log = value;
                OnPropertyChanged("Log");
            }
        }

        private DailyLogsModel _logsModel;
        public DailyLogsModel LogsModel
        {
            get => _logsModel;
            set
            {
                if (_logsModel == value) return;
                _logsModel = value;
                OnPropertyChanged("LogsModel");
            }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex == value) return;
                _selectedTabIndex = value;
                OnPropertyChanged("SelectedTabIndex");
            }
        }

        private string _logDateTime;
        public string LogDateTime
        {
            get => _logDateTime;
            set
            {
                if (_logDateTime == value) return;
                _logDateTime = value;
                OnPropertyChanged("LogDateTime");
            }
        }

        private string _logNotes;
        public string LogNotes
        {
            get => _logNotes;
            set
            {
                if (_logNotes == value) return;
                _logNotes = value;
                OnPropertyChanged("LogNotes");
            }
        }

        private BloodPressureViewModel _bloodpressureVM;
        private WeightViewModel _weightVM;

        private ObservableCollection<object> _tabCollection = new ObservableCollection<object>();
        public ObservableCollection<object> TabCollection
        {
            get => _tabCollection;
        }

        public EditLogViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            BtnSetDateTime = new RelayCommand(SetDateTime);
            BtnSaveLog = new RelayCommand(SaveLog);
            BtnCancelLog = new RelayCommand(CancelLog);

            _workspaces = workspaces;
            _conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            DailyLog dl = _conn.Table<DailyLog>().FirstOrDefault(x => x.DailyLogIsSelected == true);
            if (dl == null) return;

            _dailylogId = dl.DailyLogID;
            BuildDailyLogModel();
            Log.LogDateTime = dl.LogDateTime;
            LogDateTime = dl.LogDateTime;
            LogNotes = dl.DailyLogNotes;

            _bloodpressureVM = new BloodPressureViewModel(dl.DailyLogID);
            TabCollection.Add(_bloodpressureVM);
            _weightVM = new WeightViewModel(dl.DailyLogID);
            TabCollection.Add(_weightVM);
            SelectedTabIndex = 0;
        }

        private void CancelLog(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveLog(object obj)
        {
            _conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            DailyLog dl = _conn.Table<DailyLog>().FirstOrDefault(x => x.DailyLogID == _dailylogId);
            dl.DailyLogIsSelected = false;
            _conn.Update(dl);

            SaveBloodPressureReadings();
            LogsModel.BloodPressures = _bloodpressureVM.BloodPressures.ToList();
            SaveWeightReadings();
            LogsModel.Weights = _weightVM.Weights.ToList();

            _workspaces.Clear();
            MainViewModel workspace = new MainViewModel(_workspaces);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void SaveBloodPressureReadings()
        {
            foreach (BloodPressure bp in _bloodpressureVM.BloodPressures)
            {
                if (bp.BPID < 0)
                {
                    bp.DailyLogID = _dailylogId;
                    _conn.Insert(bp);
                }
                else
                {
                    _conn.Update(bp);
                }
            }
        }

        private void SaveWeightReadings()
        {
            foreach (Weight wt in _weightVM.Weights)
            {
                if (wt.WeightID < 0)
                {
                    wt.DailyLogId = _dailylogId;
                    _conn.Insert(wt);
                }
                else
                {
                    _conn.Update(wt);
                }
            }
        }

        private void SetDateTime(object obj)
        {
            LogDateTime = string.Format("{0:g}", DateTime.Now);
        }

        private void BuildDailyLogModel()
        {
            Log = new DailyLog();

            LogsModel = new DailyLogsModel();
            LogsModel.Logs = Log;
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
