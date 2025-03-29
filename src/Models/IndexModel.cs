namespace iesaver.Models
{
    public class IndexModel
    {
        public string Message { get; set; }
        public List<CreatureModel> PartyMembers { get; set; } = new();
        public List<CreatureModel> PreviousPartyMembers { get; set; } = new();
    }
}