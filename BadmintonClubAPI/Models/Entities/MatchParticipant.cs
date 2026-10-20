namespace BadmintonClubAPI.Models.Entities;

public class MatchParticipant
{
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
    
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    
    public int TeamNumber { get; set; }
    public bool IsWinner { get; set; }
}