namespace Sport.API.Entities;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Team { get; set; }
    public double? Result { get; set; } 
    public int CompetitionId { get; set; } 
    public Competition Competition { get; set; }
}
