using System.Reflection;
using System.Text.Json;
using IslamicCli.Data;
using IslamicCli.Utilities;

namespace IslamicCli.Command
{
    public class Adhkar
    {
        public List<Dhikr>? GetAllAdhkar()
        {
            string ResourceName = "IslamicCli.data.dhikr.json";
            Stream Stream = EmbeddedResourceReader.GetAssemblyResource(ResourceName);

            List<Dhikr> DhikrList = EmbeddedResourceReader.ReadAssemblyToJson<Dhikr>(Stream);

            if (DhikrList == null || DhikrList.Count == 0)
            {
                Console.WriteLine("No dhikr data found.");
                return null;
            }

            return DhikrList;
        }

        public Dhikr? GetRandomDhikr()
        {
            string ResourceName = "IslamicCli.data.dhikr.json";
            Stream Stream = EmbeddedResourceReader.GetAssemblyResource(ResourceName);
            List<Dhikr> DhikrList = EmbeddedResourceReader.ReadAssemblyToJson<Dhikr>(Stream);

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
    }
}
