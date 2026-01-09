using System.Reflection;
using System.Text.Json;
using IslamicCli.Data;

namespace IslamicCli.Utilities
{
    public static class EmbeddedResourceReader
    {
        public static Stream GetAssemblyResource(string ResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Stream? stream = assembly.GetManifestResourceStream(ResourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Embedded resource not found.", ResourceName);
            }

            return stream;
        }

        public static List<T> ReadAssemblyToJson<T>(Stream Stream)
        {
            using StreamReader reader = new StreamReader(Stream);
            string json = reader.ReadToEnd();

            var dhikrs = JsonSerializer.Deserialize<List<T>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return dhikrs ?? new List<T>();
        }
    }
}