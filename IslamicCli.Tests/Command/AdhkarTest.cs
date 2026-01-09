using IslamicCli.Command;
using IslamicCli.Data;
using IslamicCli.Utilities;
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
        string ResourceName = "IslamicCli.data.dhikr.json";
        Stream stream = EmbeddedResourceReader.GetAssemblyResource(ResourceName);
        List<Dhikr> list = EmbeddedResourceReader.ReadAssemblyToJson<Dhikr>(stream);

        Assert.NotNull(list);
        Assert.True(list.Count > 0);
    }
}
