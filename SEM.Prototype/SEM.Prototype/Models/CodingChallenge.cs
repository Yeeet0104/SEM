namespace SEM.Prototype.Models
{
    public class CodingChallenge
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InitialCode { get; set; }
        public string ExpectedOutput { get; set; }
        public string Difficulty { get; set; }
    }
}
