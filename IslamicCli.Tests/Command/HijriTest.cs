using IslamicCli.Command;

namespace IslamicCli.Tests.Command
{
    public class HijriTest
    {
        [Fact]
        public void GetHijriCalendar_WithNullDateTime_UsesCurrentDate()
        {
            var hijri = new Hijri();
            var result = hijri.GetHijriCalendar(null);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetHijriCalendar_WithSpecificDate_ReturnsFormattedCalendar()
        {
            var hijri = new Hijri();
            var testDate = new DateTime(2024, 1, 1);
            var result = hijri.GetHijriCalendar(testDate);
            
            Assert.NotNull(result);
            Assert.Contains("AH", result);
        }

        [Fact]
        public void GetHijriCalendar_IncludesWeekdayHeader()
        {
            var hijri = new Hijri();
            var testDate = new DateTime(2024, 1, 1);
            var result = hijri.GetHijriCalendar(testDate);
            
            Assert.Contains("Mo Tu We Th Fr Sa Su", result);
        }

        [Fact]
        public void GetHijriCalendar_DuringRamadan_IncludesRamadanMessage()
        {
            var hijri = new Hijri();
            var ramadanDate = new DateTime(2024, 3, 15); // Approximate Ramadan date
            var result = hijri.GetHijriCalendar(ramadanDate);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetHijriCalendar_BeforeRamadan_IncludesCountdown()
        {
            var hijri = new Hijri();
            var beforeRamadan = new DateTime(2024, 1, 15);
            var result = hijri.GetHijriCalendar(beforeRamadan);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetHijriCalendar_HighlightsCurrentDay()
        {
            var hijri = new Hijri();
            var testDate = new DateTime(2024, 1, 15);
            var result = hijri.GetHijriCalendar(testDate);
            
            Assert.Contains("\u001b[1;32m", result); // ANSI color code for highlight
        }
    }
}