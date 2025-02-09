namespace Sport.API.Entities;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Team { get; set; }
    public int TotalScore { get; set; } = 0;
    public ICollection<Competition> Competitions { get; set; }
}
