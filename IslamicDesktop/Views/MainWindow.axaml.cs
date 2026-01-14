using Avalonia.Controls;
using Avalonia.Threading;
using IslamicCli.Command.Prayer;
using IslamicCli.Command;
using System;
using System.Threading.Tasks;

namespace IslamicDesktop.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private DateTime _nextPrayerTime;
        private string _nextPrayerName;

        public MainWindow()
        {
            InitializeComponent();
            LoadNextPrayer();
            StartCountdown();
        }

        private async void LoadNextPrayer()
        {
            var pray = new Pray(new PrayerTimeService());
            var result = await pray.Next();

            _nextPrayerTime = result.NextPrayerTime ?? 
                              DateTime.Now.Date.AddDays(1) + result.FirstPrayerTimeTomorrow;

            _nextPrayerName = result.NextPrayerName ?? result.FirstPrayerNameTomorrow;

            PrayerNameText.Text = _nextPrayerName;
            PrayerTimeText.Text = _nextPrayerTime.ToString("HH:mm");
        }

        private void StartCountdown()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (_, _) =>
            {
                var remaining = _nextPrayerTime - DateTime.Now;

                if (remaining <= TimeSpan.Zero)
                {
                    CountdownText.Text = "It's time!";
                    ShowNotification();
                    LoadNextPrayer();
                }
                else
                {
                    CountdownText.Text =
                        $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                }
            };

            _timer.Start();
        }

        private void ShowNotification()
        {
            var notifier = new Notify(new Pray(new PrayerTimeService()), DateTime.Now);
            notifier.Start();
        }

        private void RefreshPrayer(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            LoadNextPrayer();
        }
    }
}

