namespace Sport.API.Entities;

public class Competition
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime RegistrationDeadline { get; set; } 
    public string Location { get; set; }
    public List<Participant> Participants { get; set; }

    public Competition()
    {
        RegistrationDeadline = StartDate.AddDays(-2);
    }
}
