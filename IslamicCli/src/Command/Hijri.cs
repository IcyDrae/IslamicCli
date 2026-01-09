using System.Globalization;
using System.Text;

namespace IslamicCli.Command
{
    public class Hijri
    {
        private readonly string[] HijriMonths =
        {
            "Muharram",
            "Safar",
            "Rabi' al-Awwal",
            "Rabi' al-Thani",
            "Jumada al-Ula",
            "Jumada al-Akhirah",
            "Rajab",
            "Sha'ban",
            "Ramadan",
            "Shawwal",
            "Dhul Qa'dah",
            "Dhul Hijjah"
        };

        public string GetHijriCalendar(DateTime? now = null)
        {
            var hijri = new HijriCalendar();
            hijri.HijriAdjustment = -1; // optional adjustment if your local observation differs
            var today = now ?? DateTime.Now;

            int day = hijri.GetDayOfMonth(today);
            int month = hijri.GetMonth(today);
            int year = hijri.GetYear(today);

            int daysInMonth = hijri.GetDaysInMonth(year, month);

            var sb = new StringBuilder();
            sb.AppendLine($"{HijriMonths[month - 1]} {year} AH");
            sb.AppendLine();

            sb.AppendLine("Mo Tu We Th Fr Sa Su");

            DateTime firstDay = hijri.ToDateTime(year, month, 1, 0, 0, 0, 0);
            int start = ((int)firstDay.DayOfWeek + 6) % 7; // shift to Monday start

            for (int i = 0; i < start; i++)
                sb.Append("   ");

            for (int d = 1; d <= daysInMonth; d++)
            {
                if (d == day)
                    sb.Append($"\u001b[1;32m{d:00}\u001b[0m "); // highlight today
                else
                    sb.Append($"{d:00} ");

                if ((start + d) % 7 == 0)
                    sb.AppendLine();
            }

            // Ramadan countdown / day info
            int ramadanMonth = 9;
            if (month < ramadanMonth)
            {
                int daysUntilRamadan = 0;

                // sum remaining days of current month
                daysUntilRamadan += daysInMonth - day;

                // add full months until Ramadan
                for (int m = month + 1; m < ramadanMonth; m++)
                {
                    daysUntilRamadan += hijri.GetDaysInMonth(year, m);
                }

                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"\u001b[1;36mDays until Ramadan: {daysUntilRamadan}\u001b[0m"); // bright cyan
            }
            else if (month == ramadanMonth)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"\u001b[1;36mRamadan Mubarak! Today is day {day} out of {daysInMonth} of Ramadan.\u001b[0m");
            }

            return sb.ToString();
        }
    }
}
