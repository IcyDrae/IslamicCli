using IslamicCli.Http;
using Xunit;

namespace IslamicCli.Tests.Http;

public class RequestTests
{
    [Fact]
    public async Task GetPrayerTimes_ReturnsDataIntegrationTest()
    {
        var result = await Request.GetPrayerTimes();

        var prayerTimes = result.Item1;
        var city = result.City;
        var country = result.Country;

        Assert.NotNull(prayerTimes);
        Assert.NotEmpty(prayerTimes);
        Assert.False(string.IsNullOrWhiteSpace(city));
        Assert.False(string.IsNullOrWhiteSpace(country));
    }
}
