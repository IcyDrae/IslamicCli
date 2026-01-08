namespace IslamicCli.Command.Prayer
{
    public interface IPrayerTimeService
    {
        Task<(
                Dictionary<string, string>,
                string City,
                string Country
            )> GetPrayerTimes();
    }
}
