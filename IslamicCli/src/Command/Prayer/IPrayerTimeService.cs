namespace IslamicCli.Command.Prayer
{
    public interface IPrayerTimeService
    {
        Task<(
                Dictionary<string, string>,
                string City,
                string Country
            )> GetPrayerTimes();

	Task<(Dictionary<string, string>, string City, string Country)> GetTomorrow();
    }
}
