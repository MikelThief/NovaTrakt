using NovaTrakt.Classes.Helpers;
using NovaTrakt.Classes.NovaTrakt;
using NovaTrakt.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NovaTrakt.View.Controls
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        DispatcherTimer _timer = new DispatcherTimer();
        HomeViewModel hvm;

        public Player()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += new EventHandler(timer_tick);
            _timer.Start();
        }

        private void ntMediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (hvm == null)
                hvm = (HomeViewModel)DataContext;

            posSlider.Minimum = 0;
            posSlider.Maximum = hvm.CurrentPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            if (hvm == null && DataContext != null)
                hvm = (HomeViewModel)DataContext;


            if (hvm != null && hvm.CurrentPlayer != null)
            {
                posSlider.Value = hvm.CurrentPlayer.Position.TotalSeconds;

                Journey selectedTrip = hvm.SelectedTrip;
                if (Media.GetMediaState(hvm.CurrentPlayer) == MediaState.Play)
                    hvm.UpdatePlayTime();
            }
        }

        private void volSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (hvm == null && DataContext != null)
                hvm = (HomeViewModel)DataContext;

            if (hvm != null && hvm.CurrentPlayer != null)
            {
                if (e.NewValue > 0)
                {
                    hvm.CurrentPlayer.IsMuted = false;
                    hvm.CurrentPlayer.Volume = e.NewValue;
                }
                else
                    hvm.CurrentPlayer.IsMuted = true;
            }
        }

        private void posSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (hvm == null && DataContext != null)
                hvm = (HomeViewModel)DataContext;

            if (hvm != null && hvm.CurrentPlayer != null && posSlider.IsMouseCaptureWithin)
                hvm.CurrentPlayer.Position = new TimeSpan(0, 0, 0, Convert.ToInt32(posSlider.Value), 0);
        }
    }
}
