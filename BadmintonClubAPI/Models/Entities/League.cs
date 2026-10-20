namespace BadmintonClubAPI.Models.Entities;

public class League
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}