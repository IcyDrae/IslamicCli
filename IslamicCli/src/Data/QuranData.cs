namespace IslamicCli.Data
{
    public class QuranData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Transliteration { get; set; }
        public string? Translation { get; set; }
        public string? Type { get; set; }
        public int Total_Verses { get; set; }
        public List<Ayah> Verses { get; set; } = new();
    }

    public class Ayah
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Translation { get; set; }
    }
}
