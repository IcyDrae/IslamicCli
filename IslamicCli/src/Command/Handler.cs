using IslamicCli.Data;
using IslamicCli.Command.Prayer;
using System.Diagnostics;
using System.Globalization;
using System.Text;

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
                case "notify":
                    await HandleNotify();
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
                case "fasting-days":
                    HandleFastingDays();
                    break;
                case "99":
                    Handle99Names();
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
            else if (parameters.Length > 0 && parameters[0] == "--tomorrow")
            {
                await HandlePrayTomorrow();
            }
            else
            {
                Pray Pray = new Pray(new PrayerTimeService());
                (Dictionary<string, string>,
                string City,
                string Country) prayerTimes = await Pray.All();

                await PrintPrayerTimeSummary(prayerTimes, "Today's");
            }
        }

        private async Task HandlePrayTomorrow()
        {
            Pray Pray = new Pray(new PrayerTimeService());
            var Tomorrow = await Pray.Tomorrow();

            await PrintPrayerTimeSummary(Tomorrow, "Tomorrow's");
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

        private async Task HandleNotify()
        {
            var pray = new Pray(new PrayerTimeService());
            var notifier = new Notify(pray, DateTime.Now);
            notifier.Start();

            await Task.Delay(-1);
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
            string hijriDate = Hijri.GetHijriCalendar(useColors: true);
            Console.WriteLine(hijriDate);
        }

        private void HandleFastingDays()
        {
            FastingDays fastingDays = new FastingDays();
            string fastingInfo = fastingDays.GetFastingInfo();
            Console.WriteLine(fastingInfo);
        }

        private void Handle99Names()
        {
            NinetyNineNames Names = new NinetyNineNames();
            List<NinetyNineNamesData>? NamesList = Names.GetAll();

            if (NamesList != null)
            {
                StringBuilder StringBuilder = new StringBuilder();

                foreach (var Name in NamesList)
                {
                    StringBuilder.AppendLine($"{Name.Arabic} - {Name.Transliteration} - {Name.English}");
                }

                PrintScrollable(StringBuilder.ToString());
            }
        }

        private void HandleHelp()
        {
            Console.WriteLine("Islamic CLI Help:");
            Console.WriteLine("Commands:");
            Console.WriteLine("  pray               - Get today's prayer times");
            Console.WriteLine("  pray --next        - Get the next prayer time");
	        Console.WriteLine("  pray --tomorrow    - Get the prayer times for tomorrow");
            Console.WriteLine("  notify             - Runs the process in the background. Notifies 10 minutes before each prayer time using an Adhan sound and a desktop notification");
            Console.WriteLine("  dhikr              - List available dhikr");
            Console.WriteLine("  dhikr --random     - Get a random dhikr");
            Console.WriteLine("  quran <number>     - Read a Surah from the Quran");
            Console.WriteLine("  hijri              - Show the Hijri calendar for the current month with Ramadan info");
            Console.WriteLine("  fasting-days       - Show recommended fasting days");
            Console.WriteLine("  99                 - Show the 99 Beautiful Names of Allah");
            Console.WriteLine("  help               - Show this help message");
        }

        private async Task PrintPrayerTimeSummary((Dictionary<string, string>, string City, string Country) prayerTimes, string NameOfDay)
        {
            Console.WriteLine($"{NameOfDay} prayer times for {prayerTimes.City}, {prayerTimes.Country}:");
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
