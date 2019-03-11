using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Maps.MapControl.WPF;
using NovaTrakt.Classes;
using NovaTrakt.SharedComponents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NovaTrakt.ViewModel
{
    class BaseViewModel : ObservableObject
    {
        #region Fields

        private static Log Log = new Log();

        private BackgroundWorker worker = new BackgroundWorker();
        private ProgressDialogController _progressDlg;

        private ICommand _changePageCommand;
        private ICommand _clearLogFile;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public BaseViewModel()
        {
            // Add available NovaTrakt Pages
            PageViewModels.Add(new HomeViewModel()); // 0 - The Player
            PageViewModels.Add(new LogViewModel()); // 1 - The Log

            // Create the BackgroundWorker events
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public MapMode MapMode
        {
            get
            {
                if (AppSettings.getString("MapMode", "Road") == "Road")
                    return new RoadMode();
                else if (AppSettings.getString("MapMode", "Road") == "Aerial")
                    return new AerialMode();
                else
                    return new AerialMode(true);
            }
        }

        public string MapModeString
        {
            get { return AppSettings.getString("MapMode", "Road"); }
            set
            {
                AppSettings.set("MapMode", value);
                OnPropertyChanged("MapModeString");
                OnPropertyChanged("MapMode");
            }
        }

        public bool MPH
        {
            get { return AppSettings.getBool("MPH", false); }
            set
            {
                AppSettings.set("MPH", (bool)value);
                OnPropertyChanged("MPH");
            }
        }

        public ICommand ChangePageCommand
        {
            get
            {
                // Save the log to file
                Log.WriteToFile();

                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                    p => ChangeViewModel((IPageViewModel)p),
                    p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public bool VerboseLogging
        {
            get { return AppSettings.getBool("VerboseLogging", false); }
            set
            {
                AppSettings.set("VerboseLogging", value);
                OnPropertyChanged("VerboseLogging");
            }
        }

        public ICommand ClearLogFile
        {
            get
            {
                if (_clearLogFile == null)
                {
                    _clearLogFile = new RelayCommand(
                        p => _ClearLogFile());
                }

                return _clearLogFile;
            }
        }

        public ICommand Exit
        {
            get { return new RelayCommand(p => _exit()); }
        }

        public ICommand SelectInputFolder
        {
            get
            {
                HomeViewModel hvm = (HomeViewModel)PageViewModels[0];
                return new RelayCommand(p => hvm._selectInputFolder());
            }
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        private void _ClearLogFile()
        {
            Log.ClearLog();
            PageViewModels[1] = new LogViewModel();
            CurrentPageViewModel = PageViewModels[1];
        }

        private void _exit()
        {
            Environment.Exit(0);
        }

        public async void workerRun()
        {
            // Tidy the Log File & Notify User
            MetroWindow w = (MetroWindow)Application.Current.MainWindow;
            _progressDlg = await w.ShowProgressAsync("Tidying Log File", "NovaTrakt is tidying your log file.\r\nThis process may take a while if it is the first time it has been run.");
            _progressDlg.SetIndeterminate();
            worker.RunWorkerAsync();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressDlg.CloseAsync();
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.TidyFile();
        }

        #endregion
    }
}
