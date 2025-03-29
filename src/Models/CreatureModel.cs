using ii.InfinityEngine.Files;


namespace iesaver.Models
{
    public class CreatureModel
    {
        public string Name { get; set; }
        public CreFile CreFile { get; set; }
        public List<IdsFile> Identifiers { get; set; }
        public List<ItmFile> Items { get; set; }

        public string GetIdsEntry(string file, byte entry)
        {
            var ids = Identifiers.FirstOrDefault(x => x.Filename.Equals(file, StringComparison.OrdinalIgnoreCase));
            var lines = ids.contents.Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

            var line = lines.Where(w => w.StartsWith($"{entry} ")).FirstOrDefault();
            if (line != null)
            {
                return line.Substring(line.IndexOf(" ") + 1);
            }

            return string.Empty;
        }
    }
}