namespace BadmintonClubAPI.Models.Entities;

public class Player
{
    private const int RenewalMonths = 1;
    private const int StartingPoints = 0;
    private const int NumDefaultMembershipYears = 1;
    
    public Player() { }

    public Player(string firstName, string surname, string emailAddress, DateTime dateOfBirth)
    {
        FirstName = firstName;
        Surname = surname;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        RankingPoints = StartingPoints;
        MembershipStartDate = DateTime.UtcNow;
        MembershipEndDate = MembershipStartDate.AddYears(NumDefaultMembershipYears);
    }
    
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int RankingPoints { get; set; }
    
    public DateTime MembershipStartDate { get; set; }
    public DateTime MembershipEndDate { get; set; }
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public bool IsMembershipActive()
    {
        var now = DateTime.UtcNow;
        return now >= MembershipStartDate && now <= MembershipEndDate;
    }

    public bool IsMembershipCloseToExpiry()
    {
        var now = DateTime.UtcNow;
        return IsMembershipActive() && now >= MembershipEndDate.AddMonths(-RenewalMonths);
    }
}
