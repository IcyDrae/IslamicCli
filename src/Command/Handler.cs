using System.Text.Json;

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
                    HandleDhikr();
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
                Dictionary<string, string> prayerTimes = await Request.GetPrayerTimes();

                foreach (var kvp in prayerTimes)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        private async Task HandlePrayNext(string parameter)
        {
            Dictionary<string, string> prayerTimes = await Request.GetPrayerTimes();
            DateTime now = DateTime.Now;

            DateTime? nextPrayerTime = null;
            string nextPrayerName = "";
            string firstPrayerNameTomorrow = "";
            TimeSpan firstPrayerTimeTomorrow = TimeSpan.Zero;
            bool firstPrayerSet = false;

            foreach (var kvp in prayerTimes)
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

        private void HandleDhikr()
        {
            var dhikrJson = File.ReadAllText("data/dhikr.json");
            var dhikrs = JsonSerializer.Deserialize<List<Dhikr>>(dhikrJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dhikrs == null)
            {
                Console.WriteLine("No dhikr data found");
                return;
            }

            foreach (var dhikr in dhikrs)
            {
                Console.WriteLine($"{dhikr.Text} ({dhikr.Translation}) - Repeat {dhikr.Count} times");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }
        }
    }
}
