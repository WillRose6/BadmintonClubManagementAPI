using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs;
using BadmintonClubAPI.Models.DTOs.Players;
using BadmintonClubAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Services.Players;

public class PlayerService : IPlayerService
{
    private readonly BadmintonDbContext _context;
    private readonly IPasswordHasher<Player> _passwordHasher;
    
    public PlayerService(BadmintonDbContext context, IPasswordHasher<Player> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<PlayerResponseDto>> GetAllPlayersAsync()
    {
        return await _context.Players.Select(p => new PlayerResponseDto(p.Id, p.FirstName, p.Surname, p.EmailAddress, p.DateOfBirth)).ToListAsync();
    }

    public async Task<PlayerResponseDto?> GetPlayerByIdAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return null;
        }

        return new PlayerResponseDto(player.Id, player.FirstName, player.Surname, player.EmailAddress, player.DateOfBirth);
    }

    public async Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerDto createDto)
    {
        var player = new Player(createDto.FirstName, createDto.Surname, createDto.EmailAddress, createDto.DateOfBirth);

        var passwordHash = _passwordHasher.HashPassword(player, createDto.Password);
        player.PasswordHash = passwordHash;

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return new PlayerResponseDto(player.Id, player.FirstName, player.Surname, player.EmailAddress, player.DateOfBirth);
    }

    public async Task<PlayerResponseDto?> UpdatePlayerByIdAsync(int id, UpdatePlayerDto updatePlayerDto)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(updatePlayerDto.FirstName))
        {
            player.FirstName = updatePlayerDto.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(updatePlayerDto.Surname))
        {
            player.Surname = updatePlayerDto.Surname;
        }

        if (!string.IsNullOrWhiteSpace(updatePlayerDto.EmailAddress))
        {
            player.EmailAddress = updatePlayerDto.EmailAddress;
        }

        if (updatePlayerDto.DateOfBirth.HasValue)
        {
            player.DateOfBirth = updatePlayerDto.DateOfBirth.Value;
        }
        
        await _context.SaveChangesAsync();
        return new PlayerResponseDto(player.Id, player.FirstName, player.Surname, player.EmailAddress, player.DateOfBirth);
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var playerToDelete = await _context.Players.FindAsync(id);
        if (playerToDelete == null)
        {
            return false;
        }

        _context.Players.Remove(playerToDelete);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<Player?> GetByEmailAsync(string email)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.EmailAddress == email);
    }
}