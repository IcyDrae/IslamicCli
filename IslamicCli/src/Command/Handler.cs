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
                await HandlePrayNext();
            }
            else
            {
                (Dictionary<string, string>, string City, string Country) prayerTimes = await Request.GetPrayerTimes();

                await PrintPrayerTimeSummary(prayerTimes);
            }
        }

        private async Task HandlePrayNext()
        {
            var pray = new Pray();
            (DateTime? NextPrayerTime,
            string NextPrayerName,
            string FirstPrayerNameTomorrow,
            TimeSpan FirstPrayerTimeTomorrow,
            DateTime Now) = await pray.PrayNext();

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
    }
}
