namespace Sport.API.Entities;

public class Competition
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public List<Participant> Participants { get; set; }
}
