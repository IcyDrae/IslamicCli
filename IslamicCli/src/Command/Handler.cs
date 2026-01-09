using IslamicCli.Data;
using IslamicCli.Command.Prayer;
using System.Diagnostics;

namespace IslamicCli.Command
{
    internal class Handler
    {
        public async Task Execute(string[] arguments)
        {
            await ParseArguments(arguments);
        }

        private async Task ParseArguments(string[] args)
        {
            var actualArgs = args;

            actualArgs = HandleDLLArguments(actualArgs);
            HandleNoArguments(actualArgs);

            if (actualArgs.Length == 0)
            {
                return;
            }
            await HandleCommand(actualArgs[0], actualArgs[1..]);
        }

        private string[] HandleDLLArguments(string[] args)
        {
            var actualArgs = args;

            if (args.Length > 0 && args[0].EndsWith(".dll"))
            {
                actualArgs = args.Skip(1).ToArray();
            }

            return actualArgs;
        }

        private void HandleNoArguments(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command provided");
            }
        }

        public async Task HandleCommand(string command, string[] parameters)
        {
            switch (command.ToLower())
            {
                case "pray":
                    await HandlePray(parameters);
                    break;
                case "dhikr":
                    await HandleDhikr(parameters);
                    break;
                case "help":
                    HandleHelp();
                    break;
                case "quran":
                    HandleQuran(parameters);
                    break;
                case "hijri":
                    HandleHijriCalendar();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private async Task HandlePray(string[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] == "--next")
            {
                await HandlePrayNext();
            }
            else
            {
                Pray Pray = new Pray(new PrayerTimeService());
                (Dictionary<string, string>,
                string City,
                string Country) prayerTimes = await Pray.All();

                await PrintPrayerTimeSummary(prayerTimes);
            }
        }

        private async Task HandlePrayNext()
        {
            var pray = new Pray(new PrayerTimeService());
            (DateTime? NextPrayerTime,
            string NextPrayerName,
            string FirstPrayerNameTomorrow,
            TimeSpan FirstPrayerTimeTomorrow,
            DateTime Now) = await pray.Next();

            if (NextPrayerTime != null)
            {
                Console.WriteLine($"Next prayer is {NextPrayerName} at {NextPrayerTime:HH:mm}");
            }
            else
            {
                // All today's prayers passed, show the first prayer of tomorrow
                DateTime TomorrowPrayer = Now.Date.AddDays(1) + FirstPrayerTimeTomorrow;
                Console.WriteLine($"Next prayer is {FirstPrayerNameTomorrow} at {TomorrowPrayer:HH:mm}");
            }
        }

        private async Task HandleDhikr(string[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] == "--random")
            {
                await HandleRandomDhikr();
            }
            else
            {
                Adhkar Adhkar = new Adhkar();
                List<Dhikr>? AdhkarList = Adhkar.GetAllAdhkar();

                if (AdhkarList != null)
                {
                    PrintDhikrSummary(AdhkarList);
                }
            }
        }

        private async Task HandleRandomDhikr()
        {
            Adhkar Adhkar = new Adhkar();
            Dhikr? RandomDhikr = Adhkar.GetRandomDhikr();

            if (RandomDhikr == null)
            {
                Console.WriteLine("No dhikr data found.");
                return;
            }

            PrintRandomDhikrSummary(RandomDhikr);
        }

        private void HandleQuran(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please provide a Surah number.");
                return;
            }

            Quran Quran = new Quran();
            if (int.Parse(parameters[0]) > 114 || int.Parse(parameters[0]) < 1)
            {
                Console.WriteLine("Please provide a valid Surah number.");
            }
            else if (parameters.Length > 0 && int.TryParse(parameters[0], out int surahNumber))
            {
                string Surah = Quran.ReadSurah(surahNumber);
                PrintScrollable(Surah);
            }
        }

        private void HandleHijriCalendar()
        {
            Hijri Hijri = new Hijri();
            string hijriDate = Hijri.GetHijriCalendar();
            Console.WriteLine(hijriDate);
        }

        private void HandleHelp()
        {
            Console.WriteLine("Islamic CLI Help:");
            Console.WriteLine("Commands:");
            Console.WriteLine("  pray               - Get today's prayer times");
            Console.WriteLine("  pray --next        - Get the next prayer time");
            Console.WriteLine("  dhikr              - List available dhikr");
            Console.WriteLine("  dhikr --random     - Get a random dhikr");
            Console.WriteLine("  quran <number>     - Read a Surah from the Quran");
            Console.WriteLine("  help               - Show this help message");
        }

        private async Task PrintPrayerTimeSummary((Dictionary<string, string>, string City, string Country) prayerTimes)
        {
            Console.WriteLine($"Today's prayer times for {prayerTimes.City}, {prayerTimes.Country}:");
            Console.WriteLine("---------------------");
            foreach (var kvp in prayerTimes.Item1)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            Console.WriteLine();
        }

        private void PrintDhikrSummary(List<Dhikr> Adhkar)
        {
            Console.WriteLine("Available Dhikr:");
            Console.WriteLine("----------------------------------------------------------");
            foreach (var dhikr in Adhkar)
            {
                Console.WriteLine($"{dhikr.Text} ({dhikr.Translation}) - Repeat {dhikr.Count} times");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }
        }

        private void PrintRandomDhikrSummary(Dhikr RandomDhikr)
        {
            Console.WriteLine("Random Dhikr:");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"{RandomDhikr.Text} ({RandomDhikr.Translation}) - Repeat {RandomDhikr.Count} times");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine();
        }

        private void PrintScrollable(string text)
        {
            var process = new Process();
            process.StartInfo.FileName = "more";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();
            process.StandardInput.Write(text);
            process.StandardInput.Close();
            process.WaitForExit();
        }
    }
}
