namespace BadmintonClubAPI.Models.Entities;

public class Court
{
    public Court() { }

    public Court(int index)
    {
        CourtIndex = index;
        IsActive = false;
    }
    
    public int Id { get; set; }
    public int CourtIndex { get; set; } = 0;
    public bool IsActive { get; set; } = false;
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}