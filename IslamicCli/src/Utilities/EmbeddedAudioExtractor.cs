using System.Reflection;

namespace IslamicCli.Utilities
{
    public static class EmbeddedAudioExtractor
    {
        public static string ExtractToTempFile(string resourceName, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                throw new Exception($"Resource not found: {resourceName}");

            string tempPath = Path.Combine(Path.GetTempPath(), fileName);

            using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);

            return tempPath;
        }
    }
}
