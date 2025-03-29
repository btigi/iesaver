using ii.InfinityEngine.Files;


namespace iesaver.Models
{
    public class JournalModel
    {
        public string Message { get; set; }
        public List<GamJournal> JournalEntries { get; set; }
    }
}