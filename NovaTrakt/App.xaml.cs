using NovaTrakt.SharedComponents;
using NovaTrakt.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace NovaTrakt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static Log Log = new Log();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            MainWindow app = new MainWindow();
            BaseViewModel context = new BaseViewModel();
            app.DataContext = context;
            app.Show();

            // Run the command in BaseViewModel to clear the log file AFTER the window has been created.
            if (Log.LargeFile())
                context.workerRun();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.WriteToFile();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.WriteToFile();
            //ReportCrash((Exception)e.ExceptionObject);
            Environment.Exit(0);
        }

        private static void ReportCrash(Exception e)
        {
            //var reportCrash = new ReportCrash("robert.trehy@outlook.com")
            //{
            //    IncludeScreenshot = false
            //};
            //reportCrash.Send(e);
        }
    }
}
