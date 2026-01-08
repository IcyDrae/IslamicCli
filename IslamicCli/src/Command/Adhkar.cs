using System.Reflection;
using System.Text.Json;
using IslamicCli.Data;

namespace IslamicCli.Command
{
    internal class Adhkar
    {
        public List<Dhikr>? GetAllAdhkar()
        {
            Stream Stream = this.GetAssemblyResource();

            List<Dhikr> DhikrList = this.ReadAssemblyToJson(Stream);

            if (DhikrList == null || DhikrList.Count == 0)
            {
                Console.WriteLine("No dhikr data found.");
                return null;
            }

            return DhikrList;
        }

        public Dhikr? GetRandomDhikr()
        {
            Stream Stream = this.GetAssemblyResource();
            List<Dhikr> DhikrList = this.ReadAssemblyToJson(Stream);

            if (DhikrList == null || DhikrList.Count == 0)
            {
                Console.WriteLine("No dhikr data found.");
                return null;
            }

            Random Random = new Random();
            int index = Random.Next(DhikrList.Count);
            Dhikr RandomDhikr = DhikrList[index];
            return RandomDhikr;
        }

        public Stream GetAssemblyResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "IslamicCli.data.dhikr.json";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Embedded resource not found.", resourceName);
            }

            return stream;
        }

        public List<Dhikr> ReadAssemblyToJson(Stream Stream)
        {
            using StreamReader reader = new StreamReader(Stream);
            string json = reader.ReadToEnd();

            var dhikrs = JsonSerializer.Deserialize<List<Dhikr>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return dhikrs ?? new List<Dhikr>();
        }
    }
}