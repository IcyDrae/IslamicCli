using System;
using System.Globalization;
using System.Text;

namespace IslamicCli.Command
{
    public class FastingDays
    {
        private readonly HijriCalendar _hijri = new HijriCalendar();

        public string GetFastingInfo(DateTime? now = null)
        {
            var today = now ?? DateTime.Now;
            int day = _hijri.GetDayOfMonth(today);
            int month = _hijri.GetMonth(today);
            DayOfWeek weekDay = today.DayOfWeek;

            var sb = new StringBuilder();
            sb.AppendLine("=== Recommended Fasting Days ===");
            sb.AppendLine();

            // Weekly fasting
            sb.AppendLine(GetWeeklyFasting(weekDay));

            // White days (13th, 14th, 15th of Hijri month)
            sb.AppendLine(GetMonthlyWhiteDays(day));

            return sb.ToString();
        }

        private string GetWeeklyFasting(DayOfWeek weekDay)
        {
            string result = "Weekly Fasting: Monday & Thursday";

            if (weekDay == DayOfWeek.Monday || weekDay == DayOfWeek.Thursday)
                result += " \u001b[1;32m<- Recommended today!\u001b[0m";

            return result;
        }

        private string GetMonthlyWhiteDays(int hijriDay)
        {
            string result = "White Days (13th, 14th, 15th of Hijri month)";

            if (hijriDay == 13 || hijriDay == 14 || hijriDay == 15)
                result += $" \u001b[1;32m<- Recommended today!\u001b[0m";

            return result;
        }
    }
}
