using IslamicCli.Command;
using System;
using Xunit;

namespace IslamicCli.Tests.Command
{
    public class FastingDaysTest
    {
        [Fact]
        public void TestGetFastingInfo_OnMonday_ShouldRecommendToday()
        {
            // Arrange
            var fastingDays = new FastingDays();
            DateTime testDate = new DateTime(2024, 6, 3); // Monday

            // Act
            string result = fastingDays.GetFastingInfo(testDate);

            // Assert
            Assert.Contains("<- Recommended today!", result);
        }

        [Fact]
        public void TestGetFastingInfo_OnThursday_ShouldRecommendToday()
        {
            // Arrange
            var fastingDays = new FastingDays();
            DateTime testDate = new DateTime(2024, 6, 6); // Thursday

            // Act
            string result = fastingDays.GetFastingInfo(testDate);

            // Assert
            Assert.Contains("<- Recommended today!", result);
        }

        [Fact]
        public void TestGetFastingInfo_On13thHijri_ShouldRecommendToday()
        {
            // Arrange
            var fastingDays = new FastingDays();
            DateTime testDate = new DateTime(2024, 6, 17); // Corresponds to 13th Hijri month

            // Act
            string result = fastingDays.GetFastingInfo(testDate);

            // Assert
            Assert.Contains("<- Recommended today!", result);
        }
    }
}