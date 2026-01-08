using IslamicCli.Command;
using Xunit;

namespace IslamicCli.Tests.Command;

public class AdhkarTest
{
    [Fact]
    public void GetAllAdhkar_ReturnsData()
    {
        var adhkar = new Adhkar();

        var result = adhkar.GetAllAdhkar();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetRandomDhikr_ReturnsSingleDhikr()
    {
        var adhkar = new Adhkar();

        var result = adhkar.GetRandomDhikr();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Text));
    }

    [Fact]
    public void ReadAssemblyToJson_ParsesJsonCorrectly()
    {
        var adhkar = new Adhkar();
        var stream = adhkar.GetAssemblyResource();

        var list = adhkar.ReadAssemblyToJson(stream);

        Assert.NotNull(list);
        Assert.True(list.Count > 0);
    }
}
