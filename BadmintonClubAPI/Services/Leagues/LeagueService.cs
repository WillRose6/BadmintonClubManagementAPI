using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Leagues;
using BadmintonClubAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Services.Leagues;

public class LeagueService : ILeagueService
{
    private readonly BadmintonDbContext _context;

    public LeagueService(BadmintonDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LeagueResponseDto>> GetAllLeaguesAsync()
    {
        return await _context.Leagues.Select(l => new LeagueResponseDto(l.Id, l.Name, l.StartDate, l.EndDate, l.Players.Select(p => p.Id), l.Matches.Select(m => m.Id))).ToListAsync();
    }

    public async Task<LeagueResponseDto?> GetLeagueByIdAsync(int id)
    {
        return await _context.Leagues.Where(l => l.Id == id) .Select(l => new LeagueResponseDto(l.Id, l.Name, l.StartDate, l.EndDate, l.Players.Select(p => p.Id), l.Matches.Select(m => m.Id))).FirstOrDefaultAsync();
    }

public async Task<LeagueResponseDto> CreateLeague(CreateLeagueDto createLeagueDto)
    {
        var league = new League
        {
            Name = createLeagueDto.Name,
            StartDate = createLeagueDto.StartDate,
            EndDate = createLeagueDto.EndDate
        };

        _context.Leagues.Add(league);
        await _context.SaveChangesAsync();

        return new LeagueResponseDto(league.Id, league.Name, league.StartDate, league.EndDate, [], []);
    }

    public async Task<LeagueResult> AddPlayerToLeague(int leagueId, int playerId)
    {
        var league = await _context.Leagues.Include(l => l.Players).FirstOrDefaultAsync(l => l.Id == leagueId);
        if (league == null)
        {
            return LeagueResult.LeagueNotFound;
        }

        var player = await _context.Players.FindAsync(playerId);
        if (player == null)
        {
            return LeagueResult.PlayerNotFound;
        }

        if (league.Players.Any(p => p.Id == playerId)) return LeagueResult.PlayerAlreadyInLeague;

        league.Players.Add(player);
        await _context.SaveChangesAsync();

        return LeagueResult.Success;
    }

    public async Task<LeagueResult> RemovePlayerFromLeague(int leagueId, int playerId)
    {
        var league = await _context.Leagues.Include(l => l.Players).FirstOrDefaultAsync(l => l.Id == leagueId);
        if (league == null)
        {
            return LeagueResult.LeagueNotFound;
        }

        var player = league.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
        {
            var existsGlobally = await _context.Players.AnyAsync(p => p.Id == playerId);
            return existsGlobally ? LeagueResult.PlayerNotInLeague : LeagueResult.PlayerNotFound;
        }

        league.Players.Remove(player);
        await _context.SaveChangesAsync();

        return LeagueResult.Success;
    }

    public async Task<bool> DeleteLeagueById(int id)
    {
        var league = await _context.Leagues.FindAsync(id);
        if (league == null)
        {
            return false;
        }

        _context.Leagues.Remove(league);
        await _context.SaveChangesAsync();
        return true;
    }
}