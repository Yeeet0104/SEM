namespace SEM.Prototype.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string Description { get; set; }
        public bool AllDay { get; set; }  // Add this field
    }
}
