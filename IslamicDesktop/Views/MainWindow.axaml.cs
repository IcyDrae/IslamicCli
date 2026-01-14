using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using IslamicCli.Command.Prayer;
using IslamicCli.Command;
using System;

namespace IslamicDesktop.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer = null!;
        private DateTime _nextPrayerTime;
        private string _nextPrayerName = "";

        public MainWindow()
        {
            InitializeComponent();
            LoadPrayerTimesView(); // load default view
            SetupTrayIcon();

            this.Closing += (s, e) =>
            {
                e.Cancel = true; // prevent actual close
                this.Hide();     // just hide the window
            };

            // Restore window when app is activated (dock icon clicked)
            this.Activated += (s, e) =>
            {
                if (!this.IsVisible)
                    this.Show();
            };
        }

        private void SetupTrayIcon()
        {
            var tray = new TrayIcon
            {
                ToolTipText = "Islamic App",
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://IslamicDesktop/Assets/avalonia-logo.ico")))
            };

            var showItem = new NativeMenuItem("Show App");
            showItem.Click += (_, _) => this.Show();

            var hideItem = new NativeMenuItem("Hide");
            hideItem.Click += (_, _) => this.Hide();

            var quitItem = new NativeMenuItem("Quit");
            quitItem.Click += (_, _) =>
            {
                (Application.Current!.ApplicationLifetime
                 as IClassicDesktopStyleApplicationLifetime)!.Shutdown();
            };

            tray.Menu = new NativeMenu
            {
                Items =
                {
                    showItem,
                    hideItem,
                    new NativeMenuItemSeparator(),
                    quitItem
                }
            };
        }

        // ========================
        // Parameterless logic methods
        // ========================
        private async void LoadPrayerTimesView()
        {
            var pray = new Pray(new PrayerTimeService());
            var result = await pray.All(); // get all prayer times

            var prayerTimes = result.Item1; // Dictionary<string,string>
            var city = result.City;
            var country = result.Country;

            var stack = new StackPanel { Spacing = 10 };

            stack.Children.Add(new TextBlock
            {
                Text = $"Prayer Times - {city}, {country}",
                FontSize = 20,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            });

            foreach (var kvp in prayerTimes)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = $"{kvp.Key}: {kvp.Value}",
                    FontSize = 16,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });
            }

            MainContent.Content = stack;
        }

        private void ShowDhikr()
        {
            var dhikrText = new TextBlock
            {
                Text = string.Join("\n",
                    new Adhkar().GetAllAdhkar()
                        ?.ConvertAll(d => $"{d.Text} - ({d.Translation}) {d.Count} - Times")
                        .ToArray() ?? Array.Empty<string>()
                ),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap, // <- THIS IS IMPORTANT
                FontSize = 16,
                Margin = new Avalonia.Thickness(10)
            };
            MainContent.Content = dhikrText;
        }

        private void ShowRandomDhikr()
        {
            var randomDhikr = new Adhkar().GetRandomDhikr();
            var dhikrText = new TextBlock
            {
                Text = randomDhikr != null 
                    ? $"{randomDhikr.Text} ({randomDhikr.Translation}) x{randomDhikr.Count}" 
                    : "No dhikr found"
            };
            MainContent.Content = dhikrText;
        }

        private void ShowQuran()
        {
            // Create the input box and button
            var inputBox = new TextBox
            {
                Watermark = "Enter Surah number",
                Width = 200,
                Margin = new Avalonia.Thickness(0, 0, 0, 10)
            };

            var searchButton = new Button
            {
                Content = "Search Surah",
                Width = 120,
                Margin = new Avalonia.Thickness(0, 0, 0, 10)
            };

            // TextBlock to display the Surah
            var resultText = new TextBlock
            {
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                FontSize = 16
            };

            // Button click handler
            searchButton.Click += (_, _) =>
            {
                if (int.TryParse(inputBox.Text, out int surahNumber))
                {
                    var quran = new Quran();
                    var surahText = quran.ReadSurah(surahNumber);
                    resultText.Text = string.IsNullOrEmpty(surahText) 
                        ? "Surah not found." 
                        : surahText;
                }
                else
                {
                    resultText.Text = "Please enter a valid number!";
                }
            };

            // StackPanel to arrange them vertically
            var stack = new StackPanel { Spacing = 10 };
            stack.Children.Add(inputBox);
            stack.Children.Add(searchButton);
            stack.Children.Add(resultText);

            // Wrap the stack in a ScrollViewer
            MainContent.Content = new ScrollViewer
            {
                Content = stack,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
        }


        private void ShowHijri()
        {
            var hijri = new Hijri();
            MainContent.Content = new TextBlock
            {
                Text = hijri.GetHijriCalendar()
            };
        }

        private void ShowFastingDays()
        {
            var fasting = new FastingDays();
            MainContent.Content = new TextBlock
            {
                Text = fasting.GetFastingInfo()
            };
        }

        private void Show99Names()
        {
            var names = new NinetyNineNames().GetAll();
            MainContent.Content = new TextBlock
            {
                Text = names != null 
                    ? string.Join("\n", names.ConvertAll(n => $"{n.Arabic} - {n.English} - {n.Transliteration}")) 
                    : "No names found"
            };
        }

        // ========================
        // Event Handlers for buttons
        // ========================
        private void ShowPrayerTimes(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => LoadPrayerTimesView();
        private void ShowNextPrayer(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => LoadNextPrayerView();
        private void ShowDhikr(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ShowDhikr();
        private void ShowRandomDhikr(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ShowRandomDhikr();
        private void ShowQuran(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ShowQuran();
        private void ShowHijri(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ShowHijri();
        private void ShowFastingDays(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ShowFastingDays();
        private void Show99Names(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Show99Names();

        // ========================
        // Prayer Time View
        // ========================
        private async void LoadNextPrayerView(bool onlyNext = false)
        {
            var pray = new Pray(new PrayerTimeService());
            var result = await pray.Next();

            _nextPrayerTime = result.NextPrayerTime ?? 
                              DateTime.Now.Date.AddDays(1) + result.FirstPrayerTimeTomorrow;

            _nextPrayerName = result.NextPrayerName ?? result.FirstPrayerNameTomorrow;

            var stack = new StackPanel { Spacing = 10 };

            if (!onlyNext)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = "Next Prayer",
                    FontSize = 22,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });

                stack.Children.Add(new TextBlock
                {
                    Text = _nextPrayerName,
                    FontSize = 18,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });

                stack.Children.Add(new TextBlock
                {
                    Text = _nextPrayerTime.ToString("HH:mm"),
                    FontSize = 16,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });
            }
            else
            {
                stack.Children.Add(new TextBlock
                {
                    Text = $"Next Prayer: {_nextPrayerName} at {_nextPrayerTime:HH:mm}",
                    FontSize = 20,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                });
            }

            MainContent.Content = stack;

            StartCountdown(stack);
        }

        private void StartCountdown(StackPanel stack)
        {
            _timer?.Stop();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            var countdownText = new TextBlock
            {
                FontSize = 24,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            stack.Children.Add(countdownText);

            _timer.Tick += (_, _) =>
            {
                var remaining = _nextPrayerTime - DateTime.Now;
                countdownText.Text = remaining <= TimeSpan.Zero ? "It's time!" :
                                     $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            };

            _timer.Start();
        }
    }
}
