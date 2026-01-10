using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IslamicCli.Command.Prayer;
using IslamicCli.Utilities;

namespace IslamicCli.Command
{
    public class Notify
    {
        private static bool _isRunning = false;
        private readonly Pray _pray;
        private readonly string _audioFilePath;
        private readonly HashSet<string> _notifiedPrayers = new();
        private readonly DateTime dateTime;

        public Notify(Pray prayService, DateTime? dateTime)
        {
            _pray = prayService;
            this.dateTime = dateTime ?? DateTime.Now;
            _audioFilePath = EmbeddedAudioExtractor.ExtractToTempFile(
            "IslamicCli.data.adhan.mp3",
            "islamiccli_adhan.mp3"
        );
        }

        public void Start()
        {
            if (_isRunning)
            {
                Console.WriteLine("Prayer notifications are already running.");
                return;
            }

            _isRunning = true;
            Console.WriteLine("Prayer notifier started! Running in background...");

            // Start background task
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await WaitForNextPrayerNotification();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Notifier error: {ex.Message}");
                        await Task.Delay(60000);
                    }
                }
            });
        }

        private async Task WaitForNextPrayerNotification()
        {
            var nextPrayerInfo = await _pray.Next();

            if (nextPrayerInfo.NextPrayerTime == null)
            {
                await Task.Delay(60000); // wait 1 minute before checking again
                return;
            }

            // If we've already notified this prayer, skip
            if (_notifiedPrayers.Contains(nextPrayerInfo.NextPrayerName))
            {
                await Task.Delay(60000);
                return;
            }

            DateTime notifyTime = nextPrayerInfo.NextPrayerTime.Value.AddMinutes(-10);
            TimeSpan delay = notifyTime - DateTime.Now;

            if (delay < TimeSpan.Zero)
            {
                // If we missed the notification, just trigger it immediately
                delay = TimeSpan.Zero;
            }

            int hours = (int)delay.TotalHours;
            int minutes = delay.Minutes;
            string timestamp = DateTime.Now.ToString("HH:mm");

            Console.WriteLine(
                $"[{timestamp}] Next notification for {nextPrayerInfo.NextPrayerName} at {nextPrayerInfo.NextPrayerTime:HH:mm} " +
                $"in {hours}h {minutes}m..."
            );
            Console.Out.Flush();

            await Task.Delay(delay);

            // Trigger notification
            NotifyWithNotification(nextPrayerInfo.NextPrayerName, nextPrayerInfo.NextPrayerTime.Value);

            // Mark as notified
            _notifiedPrayers.Add(nextPrayerInfo.NextPrayerName);
        }

        private void NotifyWithNotification(string prayerName, DateTime prayerTime)
        {
            string message = $"Next prayer: {prayerName} at {prayerTime:HH:mm} (in 10 minutes!)";

            // 1. Terminal bell + message
            Console.WriteLine($"\a{message}");

            // 2. Play audio file
            PlaySound(_audioFilePath);

            // 3. System notification
            ShowSystemNotification(message);
        }

        private void PlaySound(string filePath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    using var player = new System.Media.SoundPlayer(filePath);
                    player.Play();
                }
                else
                if (OperatingSystem.IsMacOS())
                {
                    var psi = new ProcessStartInfo("afplay", $"\"{filePath}\"")
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                    Process.Start(psi);
                }
                else if (OperatingSystem.IsLinux())
                {
                    if (File.Exists("/usr/bin/paplay"))
                        Process.Start("paplay", filePath);
                }
            }
            catch
            {
                // fallback: ignore if sound fails
            }
        }

        private void ShowSystemNotification(string message)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // fallback: powershell notification (requires BurntToast module installed)
                    Process.Start("powershell",
                        $"-Command \"[reflection.assembly]::loadwithpartialname('System.Windows.Forms');" +
                        $"[System.Windows.Forms.MessageBox]::Show('{message}', 'IslamicCli')\"");
                }
                else if (OperatingSystem.IsMacOS())
                {
                    // Escape quotes for AppleScript
                    var escapedMessage = message.Replace("\"", "\\\"");
                    
                    // Use osascript to show a tiny dialog window
                    var psi = new ProcessStartInfo("osascript", 
                        $"-e \"display dialog \\\"{escapedMessage}\\\" buttons {{\\\"OK\\\"}} default button \\\"OK\\\" with title \\\"IslamicCli\\\"\"")
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("notify-send", $"\"IslamicCli\" \"{message}\"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification failed: {ex.Message}");
            }
        }
    }
}
