using IslamicCli.Http;

namespace IslamicCli.Command.Prayer
{
    public class PrayerTimeService : IPrayerTimeService
    {
        public Task<(Dictionary<string, string>, string City, string Country)> GetPrayerTimes()
        {
            return Request.GetPrayerTimesAll();
        }

	    public Task<(Dictionary<string, string>, string City, string Country)> GetTomorrow()
	    {
            return Request.GetPrayerTimesTomorrow();
        }
    }
}
