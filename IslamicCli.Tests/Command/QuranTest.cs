using IslamicCli.Command;
using Xunit;

namespace IslamicCli.Tests.Command
{
    public class QuranTests
    {
        [Fact]
        public void ReadSurah_WithValidId_ReturnsSurahText()
        {
            // Arrange
            var quran = new Quran();

            // Act
            string result = quran.ReadSurah(1);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Contains("Al-Fatihah", result);
        }

        [Fact]
        public void ReadSurah_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var quran = new Quran();

            // Act
            string result = quran.ReadSurah(115);

            // Assert
            Assert.Equal("Surah not found.", result);
        }
    }
}
