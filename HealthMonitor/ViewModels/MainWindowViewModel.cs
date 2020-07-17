using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace HealthMonitor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public ICommand MenuMain { get; set; }
        public ICommand MenuExit { get; set; }
        public ICommand MenuAddLog { get; set; }
        public ICommand MenuEditLog { get; set; }
        public ICommand MenuDeleteLog { get; set; }
        public ICommand MenuAddNew { get; set; }

        public MainWindowViewModel()
        {
            MenuMain = new RelayCommand(ShowMain);
            MenuExit = new RelayCommand(ExitHealthLogs);
            MenuAddLog = new RelayCommand(AddNewLog);
            MenuEditLog = new RelayCommand(EditLog);
            MenuDeleteLog = new RelayCommand(DeleteLog);
            MenuAddNew = new RelayCommand(ShowAddNew);
            ShowMainWindow();
        }

        private void AddNewLog(object obj)
        {
            Workspaces.Clear();
            AddLogViewModel workspace = new AddLogViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void EditLog(object obj)
        {
            Workspaces.Clear();
            EditLogViewModel workspace = new EditLogViewModel(_workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void DeleteLog(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowAddNew(object obj)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ViewModelBase> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<ViewModelBase>();
                    _workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void ShowMain(object obj)
        {
            ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            Workspaces.Clear();
            MainViewModel workspace = new MainViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void ExitHealthLogs(object obj)
        {
            App.Current.MainWindow.Close();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
