using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Matches;
using BadmintonClubAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Services.Matches;

public class MatchService : IMatchService
{
    private readonly BadmintonDbContext _context;

    public MatchService(BadmintonDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync()
    {
        return await _context.Matches.Include(m => m.MatchParticipants).Select(m => new MatchResponseDto(m.Id, m.StartDate, m.EndDate, m.LeagueId, m.MatchParticipants.Select(p => new MatchParticipantDto(p.PlayerId, p.TeamNumber, p.IsWinner)).ToList())).ToListAsync();
    }

    public async Task<MatchResponseDto?> GetMatchByIdAsync(int id)
    {
        var match = await _context.Matches.Include(m => m.MatchParticipants).FirstOrDefaultAsync(m => m.Id == id);
        return match == null ? null : new MatchResponseDto(match.Id, match.StartDate, match.EndDate, match.LeagueId, match.MatchParticipants.Select(p => new MatchParticipantDto(p.PlayerId, p.TeamNumber, p.IsWinner)).ToList());
    }

    public async Task<IEnumerable<MatchResponseDto>> GetMatchesByLeagueIdAsync(int leagueId)
    {
        return await _context.Matches.Include(m => m.MatchParticipants).Where(m => m.LeagueId == leagueId).Select(m => new MatchResponseDto(m.Id, m.StartDate, m.EndDate, m.LeagueId, m.MatchParticipants.Select(p => new MatchParticipantDto(p.PlayerId, p.TeamNumber, p.IsWinner)).ToList())).ToListAsync();
    }

    public async Task<(MatchResult Result, MatchResponseDto? Match)> CreateMatchAsync(CreateMatchDto createMatchDto)
    {
        if (createMatchDto.LeagueId.HasValue)
        {
            var leagueExists = await _context.Leagues.AnyAsync(l => l.Id == createMatchDto.LeagueId.Value);
            if (!leagueExists) return (MatchResult.LeagueNotFound, null);
        }

        foreach (var p in createMatchDto.Participants)
        {
            var playerExists = await _context.Players.AnyAsync(pl => pl.Id == p.PlayerId);
            if (!playerExists)
            {
                return (MatchResult.PlayerNotFound, null);
            }
        }

        var match = new Match
        {
            StartDate = createMatchDto.StartDate,
            EndDate = createMatchDto.EndDate,
            LeagueId = createMatchDto.LeagueId,
            MatchParticipants = createMatchDto.Participants.Select(p => new MatchParticipant
            {
                PlayerId = p.PlayerId,
                TeamNumber = p.TeamNumber,
                IsWinner = p.IsWinner
            }).ToList()
        };

        _context.Matches.Add(match);
        await _context.SaveChangesAsync();

        var response = new MatchResponseDto(
            match.Id,
            match.StartDate,
            match.EndDate,
            match.LeagueId,
            createMatchDto.Participants);

        return (MatchResult.Success, response);
    }

    public async Task<bool> DeleteMatchAsync(int id)
    {
        var match = await _context.Matches.FindAsync(id);
        if (match == null)
        {
            return false;
        }

        _context.Matches.Remove(match);
        await _context.SaveChangesAsync();
        return true;
    }
}