using IslamicCli.Tests.FakeServices;
using IslamicCli.Command.Prayer;
using Xunit;

namespace IslamicCli.Tests.Command;

public class PrayTest
{
    [Fact]
    public async Task All_Returns_PrayerTimes_And_Location()
    {
        var fakeService = new FakePrayerTimeService();
        var pray = new Pray(fakeService);

        var result = await pray.All();

        Assert.NotNull(result.Item1);
        Assert.Equal("Tirana", result.City);
        Assert.Equal("Albania", result.Country);
        Assert.True(result.Item1.ContainsKey("Fajr"));
        Assert.True(result.Item1.ContainsKey("Dhuhr"));
        Assert.True(result.Item1.ContainsKey("Asr"));
        Assert.True(result.Item1.ContainsKey("Maghrib"));
        Assert.True(result.Item1.ContainsKey("Isha"));
    }

    [Fact]
    public async Task NextPrayer_ShouldReturn_Dhuhr_WhenTimeIs10AM()
    {
        var fakeService = new FakePrayerTimeService();
        var fixedTime = new DateTime(2026, 1, 8, 10, 0, 0);

        var pray = new Pray(fakeService, () => fixedTime);

        var result = await pray.Next();

        Assert.Equal("Dhuhr", result.NextPrayerName);
        Assert.Equal(new DateTime(2026, 1, 8, 12, 0, 0), result.NextPrayerTime);
    }
}
