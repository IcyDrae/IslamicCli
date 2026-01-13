using System.Text.Json;

namespace IslamicCli.Http
{
    public class Request
    {
        private static async Task<(double Latitude, double Longitude, string City, string Country)> GetLocation()
        {
            string JsonResponse = await Make("http://ip-api.com/json/");
            using JsonDocument location = JsonDocument.Parse(JsonResponse);

            double Latitude = location.RootElement.GetProperty("lat").GetDouble();
            double Longitude = location.RootElement.GetProperty("lon").GetDouble();
            string City = location.RootElement.GetProperty("city").GetString() ?? string.Empty;
            string Country = location.RootElement.GetProperty("country").GetString() ?? string.Empty;

            return (Latitude, Longitude, City, Country);
        }

        public static async Task<(Dictionary<string, string>, string City, string Country)> GetPrayerTimesAll()
        {
            var coords = await GetLocation();
            double latitude = coords.Latitude;
            double longitude = coords.Longitude;
            string url = $"https://api.aladhan.com/timings/now?latitude={latitude}&longitude={longitude}&method=3";

            return await GetPrayerTimes(url, coords);
        }

	    public static async Task<(Dictionary<string, string>, string City, string Country)> GetPrayerTimesTomorrow()
        {
            // Get date of tomorrow
            DateTime Today = DateTime.Now;
            DateTime Tomorrow = Today.AddDays(1);
            string TomorrowFormatted = Tomorrow.ToUniversalTime().ToString("dd-MM-yyyy");
            var coords = await GetLocation();
            double latitude = coords.Latitude;
            double longitude = coords.Longitude;
            string url = $"https://api.aladhan.com/timings/{TomorrowFormatted}?latitude={latitude}&longitude={longitude}&method=3";

            return await GetPrayerTimes(url, coords);
        }

        public static async Task<(Dictionary<string, string>, string City, string Country)> GetPrayerTimes(string Url, (double Latitude, double Longitude, string City, string Country) Coordinates)
        {
            string JsonResponse = await Make(Url);
            using JsonDocument prayerData = JsonDocument.Parse(JsonResponse);
            JsonElement timings = prayerData.RootElement.GetProperty("data").GetProperty("timings");

            Dictionary<string, string> prayerTimes = new Dictionary<string, string>();
            foreach (var property in timings.EnumerateObject())
            {
                string key = property.Name;

                if (key == "Lastthird") key = "Last third of the night";

                if (key != "Sunset" &&
                    key != "Imsak" &&
                    key != "Firstthird")
                {
                    var prayerTime = property.Value.GetString();
                    if (prayerTime != null)
                    {
                        prayerTimes.Add(key, prayerTime);
                    }
                }
            }

            return (prayerTimes, Coordinates.City, Coordinates.Country);
        }

        private static async Task<string> Make(string url)
        {
            using HttpClient client = new HttpClient();

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error:");
                Console.WriteLine(e.Message);
                throw;
            }
            
        }
    }
}
