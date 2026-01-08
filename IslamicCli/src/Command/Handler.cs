using System.Reflection;
using System.Text.Json;
using IslamicCli.Http;
using IslamicCli.Data;

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
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private async Task HandlePray(string[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] == "--next")
            {
                await HandlePrayNext(parameters[0]);
            }
            else
            {
                (Dictionary<string, string>, string City, string Country) prayerTimes = await Request.GetPrayerTimes();

                await PrintPrayerTimeSummary(prayerTimes);
            }
        }

        private async Task HandlePrayNext(string parameter)
        {
            (Dictionary<string, string>, string City, string Country) prayerTimes = await Request.GetPrayerTimes();
            DateTime now = DateTime.Now;

            DateTime? nextPrayerTime = null;
            string nextPrayerName = "";
            string firstPrayerNameTomorrow = "";
            TimeSpan firstPrayerTimeTomorrow = TimeSpan.Zero;
            bool firstPrayerSet = false;

            foreach (var kvp in prayerTimes.Item1)
            {
                // Skip unwanted keys for "next prayer" calculation
                if (kvp.Key == "Sunrise" || kvp.Key == "Midnight" || kvp.Key == "Last third of the night")
                    continue;

                if (TimeSpan.TryParse(kvp.Value, out TimeSpan prayerTimeSpan))
                {
                    DateTime prayerTime = now.Date + prayerTimeSpan;

                    if (!firstPrayerSet)
                    {
                        // Save the first prayer of the day (to wrap to tomorrow if needed)
                        firstPrayerNameTomorrow = kvp.Key;
                        firstPrayerTimeTomorrow = prayerTimeSpan;
                        firstPrayerSet = true;
                    }

                    if (prayerTime > now)
                    {
                        nextPrayerTime = prayerTime;
                        nextPrayerName = kvp.Key;
                        break; // found the next prayer today
                    }
                }
            }

            if (nextPrayerTime != null)
            {
                Console.WriteLine($"Next prayer is {nextPrayerName} at {nextPrayerTime:HH:mm}");
            }
            else
            {
                // All today's prayers passed, show the first prayer of tomorrow
                DateTime tomorrowPrayer = now.Date.AddDays(1) + firstPrayerTimeTomorrow;
                Console.WriteLine($"Next prayer is {firstPrayerNameTomorrow} at {tomorrowPrayer:HH:mm}");
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
                Stream Stream = GetAssemblyResource();

                List<Dhikr> DhikrList = ReadAssemblyToJson(Stream);

                if (DhikrList == null || DhikrList.Count == 0)
                {
                    Console.WriteLine("No dhikr data found.");
                    return;
                }

                PrintDhikrSummary(DhikrList);
            }
        }

        private async Task HandleRandomDhikr()
        {
            Stream Stream = GetAssemblyResource();
            List<Dhikr> DhikrList = ReadAssemblyToJson(Stream);

            if (DhikrList == null || DhikrList.Count == 0)
            {
                Console.WriteLine("No dhikr data found.");
                return;
            }

            Random Random = new Random();
            int index = Random.Next(DhikrList.Count);
            Dhikr RandomDhikr = DhikrList[index];

            Console.WriteLine("Random Dhikr:");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"{RandomDhikr.Text} ({RandomDhikr.Translation}) - Repeat {RandomDhikr.Count} times");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine();
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

        private void PrintDhikrSummary(List<Dhikr> dhikrs)
        {
            Console.WriteLine("Available Dhikr:");
            Console.WriteLine("----------------------------------------------------------");
            foreach (var dhikr in dhikrs)
            {
                Console.WriteLine($"{dhikr.Text} ({dhikr.Translation}) - Repeat {dhikr.Count} times");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }
        }

        private Stream GetAssemblyResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "IslamicCli.data.dhikr.json";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Embedded resource not found.", resourceName);
            }

            return stream;
        }

        private List<Dhikr> ReadAssemblyToJson(Stream Stream)
        {
            using StreamReader reader = new StreamReader(Stream);
            string json = reader.ReadToEnd();

            var dhikrs = JsonSerializer.Deserialize<List<Dhikr>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return dhikrs ?? new List<Dhikr>();
        }
    }
}
