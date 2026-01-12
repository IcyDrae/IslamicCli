using IslamicCli.Http;

namespace IslamicCli.Command.Prayer
{
    public class Pray
    {
        private readonly IPrayerTimeService _service;
        private readonly Func<DateTime> _now;

        public Pray(IPrayerTimeService service, Func<DateTime>? now = null)
        {
            _service = service;
            _now = now ?? (() => DateTime.Now);
        }

        public async Task<(
            Dictionary<string, string>,
            string City,
            string Country
        )> All()
        {
            return await _service.GetPrayerTimes();
        }

	public async Task<(Dictionary <string, string>, string City, string Country)> Tomorrow()
	{
            return await _service.GetTomorrow();
        }

        public async Task<(DateTime? NextPrayerTime,
                            string NextPrayerName,
                            string FirstPrayerNameTomorrow,
                            TimeSpan FirstPrayerTimeTomorrow,
                            DateTime Now)> Next()
        {
            var prayerTimes = await _service.GetPrayerTimes();
            DateTime Now = _now();

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
