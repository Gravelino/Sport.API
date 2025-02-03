namespace Sport.API.Entities;

public class ParticipantCompetition
{
    public int Id { get; set; }
    public int CompetitionId { get; set; }
    public Competition Competition { get; set; }
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }
    public int ParticipantScore { get; set; }
}