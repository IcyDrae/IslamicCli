using System.Globalization;
using System.Text;

namespace IslamicCli.Command
{
    public class Hijri
    {
        private readonly string[] HijriMonths =
        {
            "Muharram", "Safar", "Rabi' al-Awwal", "Rabi' al-Thani",
            "Jumada al-Ula", "Jumada al-Akhirah", "Rajab", "Sha'ban",
            "Ramadan", "Shawwal", "Dhul Qa'dah", "Dhul Hijjah"
        };

        private readonly HijriCalendar _hijri = new HijriCalendar();

        public Hijri()
        {
            _hijri.HijriAdjustment = -1;
        }

        public string GetHijriCalendar(DateTime? now = null)
        {
            var today = now ?? DateTime.Now;
            int day = _hijri.GetDayOfMonth(today);
            int month = _hijri.GetMonth(today);
            int year = _hijri.GetYear(today);
            int daysInMonth = _hijri.GetDaysInMonth(year, month);

            var sb = new StringBuilder();
            sb.AppendLine(GetHeader(month, year));
            sb.AppendLine();
            sb.AppendLine(GetWeekDaysHeader());
            sb.Append(BuildCalendarDays(day, month, year, daysInMonth));
            sb.Append(BuildRamadanInfo(day, month, year, daysInMonth));

            return sb.ToString();
        }

        private string GetHeader(int month, int year)
        {
            return $"{HijriMonths[month - 1]} {year} AH";
        }

        private string GetWeekDaysHeader()
        {
            return "Mo Tu We Th Fr Sa Su";
        }

        private string BuildCalendarDays(int today, int month, int year, int daysInMonth)
        {
            var sb = new StringBuilder();

            DateTime firstDay = _hijri.ToDateTime(year, month, 1, 0, 0, 0, 0);
            int start = ((int)firstDay.DayOfWeek + 6) % 7; // shift to Monday start

            sb.Append(new string(' ', start * 3));

            for (int d = 1; d <= daysInMonth; d++)
            {
                if (d == today)
                    sb.Append($"\u001b[1;32m{d:00}\u001b[0m ");
                else
                    sb.Append($"{d:00} ");

                if ((start + d) % 7 == 0)
                    sb.AppendLine();
            }

            sb.AppendLine();
            return sb.ToString();
        }

        private string BuildRamadanInfo(int day, int month, int year, int daysInMonth)
        {
            var sb = new StringBuilder();
            int ramadanMonth = 9;

            if (month < ramadanMonth)
            {
                int daysUntilRamadan = CalculateDaysUntilRamadan(day, month, year, ramadanMonth);
                sb.AppendLine();
                sb.AppendLine($"\u001b[1;36mDays until Ramadan: {daysUntilRamadan}\u001b[0m");
            }
            else if (month == ramadanMonth)
            {
                sb.AppendLine();
                sb.AppendLine($"\u001b[1;36mRamadan Mubarak! Today is day {day} out of {daysInMonth} of Ramadan.\u001b[0m");
            }

            return sb.ToString();
        }

        private int CalculateDaysUntilRamadan(int currentDay, int currentMonth, int year, int ramadanMonth)
        {
            int days = _hijri.GetDaysInMonth(year, currentMonth) - currentDay;
            for (int m = currentMonth + 1; m < ramadanMonth; m++)
            {
                days += _hijri.GetDaysInMonth(year, m);
            }
            return days;
        }
    }
}
