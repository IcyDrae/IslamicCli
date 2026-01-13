using IslamicCli.Utilities;
using IslamicCli.Data;

namespace IslamicCli.Command
{
    public class NinetyNineNames
    {
        public List<NinetyNineNamesData>? GetAll()
        {
            string ResourceName = "IslamicCli.data.99names.json";
            Stream Stream = EmbeddedResourceReader.GetAssemblyResource(ResourceName);

            List<NinetyNineNamesData> Names = EmbeddedResourceReader.ReadAssemblyToJson<NinetyNineNamesData>(Stream);

            if (Names == null || Names.Count == 0)
            {
                Console.WriteLine("No names data found.");
                return null;
            }

            return Names;
        }
    }
}

