using IslamicCli.Tests.FakeServices;
using IslamicCli.Command.Prayer;
using Xunit;

namespace IslamicCli.Tests.Command;

public class PrayTest
{
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
