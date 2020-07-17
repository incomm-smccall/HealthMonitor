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
    public class AddLogViewModel : ViewModelBase
    {
        public ICommand BtnSetDateTime { get; set; }
        public ICommand BtnSaveLog { get; set; }
        public ICommand BtnCancelLog { get; set; }

        private ObservableCollection<ViewModelBase> _workspaces;
        private SQLiteConnection _conn;

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

        private ObservableCollection<object> _tabCollection = new ObservableCollection<object>();
        public ObservableCollection<object> TabCollection
        {
            get { return _tabCollection; }
        }

        private BloodPressureViewModel _bloodpressureVM;
        private WeightViewModel _weightVM;

        public AddLogViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            BtnSetDateTime = new RelayCommand(SetDateTime);
            BtnSaveLog = new RelayCommand(SaveLog);
            BtnCancelLog = new RelayCommand(CancelLog);

            _workspaces = workspaces;
            _conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            LogDateTime = string.Format("{0:g}", DateTime.Now);

            //int dailyLogId = BuildDailyLogModel();
            int dailyLogId = 0;
            BuildDailyLogModel();

            _bloodpressureVM = new BloodPressureViewModel(dailyLogId);
            TabCollection.Add(_bloodpressureVM);
            _weightVM = new WeightViewModel(dailyLogId);
            TabCollection.Add(_weightVM);
            SelectedTabIndex = 0;
        }

        private void CancelLog(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveLog(object obj)
        {
            int logId = SaveDailyLog();
            SaveBloodPressureReadings(logId);
            LogsModel.BloodPressures = _bloodpressureVM.BloodPressures.ToList();
            SaveWeightReadings(logId);
            LogsModel.Weights = _weightVM.Weights.ToList();

            _workspaces.Clear();
            MainViewModel workspace = new MainViewModel(_workspaces);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private int SaveDailyLog()
        {
            Log.LogDateTime = LogDateTime;
            Log.DailyLogNotes = LogNotes;
            Log.DailyLogIsSelected = false;
            _conn.Insert(Log);
            return Log.DailyLogID;
        }

        private void SaveBloodPressureReadings(int logId)
        {
            foreach (BloodPressure bp in _bloodpressureVM.BloodPressures)
            {
                bp.DailyLogID = logId;
                _conn.Insert(bp);
            }
        }

        private void SaveWeightReadings(int logId)
        {
            foreach (Weight wt in _weightVM.Weights)
            {
                wt.DailyLogId = logId;
                _conn.Insert(wt);
            }
        }

        private void BuildDailyLogModel()
        {
            Log = new DailyLog();
            Log.LogDateTime = string.Format("{0:g}", DateTime.Now);

            LogsModel = new DailyLogsModel();
            LogsModel.Logs = Log;
        }

        private int XXXBuildDailyLogModel()
        {
            Log = new DailyLog();
            Log.LogDateTime = string.Format("{0:g}", DateTime.Now);
            //_conn.Insert(Log);
            
            LogsModel = new DailyLogsModel();
            LogsModel.Logs = Log;
            return Log.DailyLogID;
        }

        private void SetDateTime(object obj)
        {
            LogDateTime = string.Format("{0:g}", DateTime.Now);
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
