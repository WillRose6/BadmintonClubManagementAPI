using BadmintonClubAPI.Models.Entities;

namespace BadmintonClubAPI.Tests.Entities;

public class PlayerTests
{
    [Theory]
    [InlineData(-1, 12, true)]
    [InlineData(-24, -12, false)]
    [InlineData(1, 12, false)]
    public void IsMembershipActive_ShouldReturnExpectedResult(int startOffsetMonths, int endOffsetMonths, bool expectedResult)
    {
        var now = DateTime.UtcNow;
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20))
        {
            MembershipStartDate = now.AddMonths(startOffsetMonths),
            MembershipEndDate = now.AddMonths(endOffsetMonths)
        };

        var isActive = player.IsMembershipActive();
        Assert.Equal(expectedResult, isActive);
    }

    [Theory]
    [InlineData(15, true)]
    [InlineData(5, true)]
    [InlineData(60, false)]
    [InlineData(-10, false)]
    public void IsMembershipCloseToExpiry_ShouldReturnExpectedResult(int daysUntilExpiry, bool expectedResult)
    {
        var now = DateTime.UtcNow;
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20))
        {
            MembershipStartDate = now.AddMonths(-11),
            MembershipEndDate = now.AddDays(daysUntilExpiry)
        };

        var isClose = player.IsMembershipCloseToExpiry();
        Assert.Equal(expectedResult, isClose);
    }
}