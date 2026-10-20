namespace BadmintonClubAPI.Models.Entities;

public class Match
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int? LeagueId { get; set; }
    public League? League { get; set; } = null;
    
    public ICollection<MatchParticipant> MatchParticipants { get; set; } = new List<MatchParticipant>();
}