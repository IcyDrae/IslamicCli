namespace IslamicCli.Tests.FakeServices;

using IslamicCli.Command.Prayer;

public class FakePrayerTimeService : IPrayerTimeService
{
    public Task<(Dictionary<string, string>, string City, string Country)> GetPrayerTimes()
    {
        var data = new Dictionary<string, string>
        {
            { "Fajr", "05:00" },
            { "Dhuhr", "12:00" },
            { "Asr", "15:30" },
            { "Maghrib", "18:00" },
            { "Isha", "19:30" }
        };

        return Task.FromResult((data, "Tirana", "Albania"));
    }
}
