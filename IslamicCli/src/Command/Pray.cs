using IslamicCli.Http;

namespace IslamicCli.Command
{
    internal class Pray
    {
        public async Task<(
            Dictionary<string, string>,
            string City,
            string Country
        )> All()
        {
            (Dictionary<string, string>,
            string City,
            string Country) PrayerTimes = await Request.GetPrayerTimes();

            return PrayerTimes;
        }

        public async Task<(DateTime? NextPrayerTime,
                            string NextPrayerName,
                            string FirstPrayerNameTomorrow,
                            TimeSpan FirstPrayerTimeTomorrow,
                            DateTime Now)> Next()
        {
            (Dictionary<string, string>, string City, string Country) prayerTimes = await Request.GetPrayerTimes();
            DateTime Now = DateTime.Now;

            DateTime? NextPrayerTime = null;
            string NextPrayerName = "";
            string FirstPrayerNameTomorrow = "";
            TimeSpan FirstPrayerTimeTomorrow = TimeSpan.Zero;
            bool firstPrayerSet = false;

            foreach (var kvp in prayerTimes.Item1)
            {
                // Skip unwanted keys for "next prayer" calculation
                if (kvp.Key == "Sunrise" || kvp.Key == "Midnight" || kvp.Key == "Last third of the night")
                    continue;

                if (TimeSpan.TryParse(kvp.Value, out TimeSpan prayerTimeSpan))
                {
                    DateTime prayerTime = Now.Date + prayerTimeSpan;

                    if (!firstPrayerSet)
                    {
                        // Save the first prayer of the day (to wrap to tomorrow if needed)
                        FirstPrayerNameTomorrow = kvp.Key;
                        FirstPrayerTimeTomorrow = prayerTimeSpan;
                        firstPrayerSet = true;
                    }

                    if (prayerTime > Now)
                    {
                        NextPrayerTime = prayerTime;
                        NextPrayerName = kvp.Key;
                        break; // found the next prayer today
                    }
                }
            }

            return (NextPrayerTime,
                    NextPrayerName,
                    FirstPrayerNameTomorrow,
                    FirstPrayerTimeTomorrow,
                    Now);
        }
    }
}