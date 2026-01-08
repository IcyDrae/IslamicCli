using IslamicCli.Data;
using IslamicCli.Utilities;
using System.Text;

namespace IslamicCli.Command
{
    public class Quran
    {
        public string ReadSurah(int id)
        {
            string resourceName = "IslamicCli.data.quran_en.json";
            Stream stream = EmbeddedResourceReader.GetAssemblyResource(resourceName);

            List<QuranData> surahs = EmbeddedResourceReader.ReadAssemblyToJson<QuranData>(stream);

            var surah = surahs.FirstOrDefault(s => s.Id == id);

            if (surah == null)
                return "Surah not found.";

            var sb = new StringBuilder();
            sb.AppendLine($"{surah.Transliteration} - {surah.Translation}");
            sb.AppendLine();

            foreach (var ayah in surah.Verses)
            {
                sb.AppendLine($"{ayah.Id}. {ayah.Translation}");
            }

            return sb.ToString();
        }
    }
}
