using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Windows;
using System;
using System.IO;
using NovaTrakt.Classes;
using NovaTrakt.SharedComponents;

namespace NovaTrakt.ViewModel
{
    class LogViewModel : ObservableObject, IPageViewModel
    {
        private static Log Log = new Log();

        private ProgressDialogController _progressDlg;
        private string _logText;
        private BackgroundWorker reader = new BackgroundWorker
        {
            WorkerSupportsCancellation = true
        };

        public LogViewModel()
        {
            reader.DoWork += Reader_DoWork;
            reader.RunWorkerCompleted += Reader_RunWorkerCompleted;
        }

        private async void GetLog()
        {
            if (Log.FileSizeInBytes != 0)
            {
                // Create a Progress Dialog
                MetroWindow w = (MetroWindow)Application.Current.MainWindow;
                _progressDlg = await w.ShowProgressAsync("Please wait...", "Opening Log File\r\n\r\nThis may take some time");
                _progressDlg.SetCancelable(true);
                _progressDlg.Canceled += _progressDlg_Canceled;
                _progressDlg.SetIndeterminate();

                // Run the worker
                reader.RunWorkerAsync();
            }
        }

        public string Name { get { return "Log"; } }

        public string LogText
        {
            get
            {
                if (_logText == null)
                    GetLog();

                return _logText;
            }
        }


        private void Reader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPropertyChanged("LogText");
            _progressDlg.CloseAsync();
        }
        private void Reader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Read the file to string
                foreach (string line in File.ReadLines(Log.LogPath()))
                {
                    if (reader.CancellationPending)
                    {
                        e.Cancel = true;
                        _logText += " ** User Cancelled Operation ** ";
                        return;
                    }

                    _logText += line + "\r\n";
                    OnPropertyChanged("LogText");
                }
            }
            catch (Exception ex)
            {
                _logText = " ** Unable to read file ** \r\n\r\n Exception Message: " + ex.Message;
            }
        }

        private void _progressDlg_Canceled(object sender, EventArgs e)
        {
            reader.CancelAsync();
        }
    }
}
