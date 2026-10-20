namespace BadmintonClubAPI.Models.Entities;

public class Booking
{
    public Booking() { }

    public Booking(int playerId, int courtId, DateTime startDate, DateTime endDate)
    {
        PlayerId = playerId;
        CourtId = courtId;
        StartDate = startDate;
        EndDate = endDate;
        CreatedDate = DateTime.UtcNow;
    }

    public int Id { get; set; }
    
    public int PlayerId { get; set; }
    public int CourtId { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }

    public Player Player { get; set; } = null!;
    public Court Court { get; set; } = null!;
}