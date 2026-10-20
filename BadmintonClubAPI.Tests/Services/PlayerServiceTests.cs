using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Players;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BadmintonClubAPI.Tests.Services;

public class PlayerServiceTests
{
    private BadmintonDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BadmintonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        
        return new BadmintonDbContext(options);
    }

    [Fact]
    public async Task GetAllPlayersAsync_ShouldReturnAllPlayers_AsResponseDtos()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();

        var player1 = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 05, 20));
        var player2 = new Player("Alex", "Price", "Alex.Price@gmail.com", new DateTime(1995, 02, 18));
        
        context.Players.AddRange(player1, player2);
        await context.SaveChangesAsync();
        
        var service = new PlayerService(context, mockHasher.Object);
        var result = (await service.GetAllPlayersAsync()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var dto1 = result.FirstOrDefault(p => p.FirstName == player1.FirstName);
        Assert.NotNull(dto1);
        Assert.Equal(player1.FirstName, dto1.FirstName);
        Assert.Equal(player1.Surname, dto1.Surname);
        Assert.Equal(player1.EmailAddress, dto1.EmailAddress);
        Assert.Equal(player1.DateOfBirth, dto1.DateOfBirth);
        
        var dto2 = result.FirstOrDefault(p => p.FirstName == player2.FirstName);
        Assert.NotNull(dto2);
        Assert.Equal(player2.FirstName, dto2.FirstName);
        Assert.Equal(player2.Surname, dto2.Surname);
        Assert.Equal(player2.EmailAddress, dto2.EmailAddress);
        Assert.Equal(player2.DateOfBirth, dto2.DateOfBirth);
    }
    
    [Fact]
    public async Task GetAllPlayersAsync_WhenNoPlayersExist_ShouldReturnEmptyList()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();
        var service = new PlayerService(context, mockHasher.Object);

        var result = await service.GetAllPlayersAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPlayerByIdAsync_WhenPlayerExists_ShouldReturnPlayerDto()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();

        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 05, 20));
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new PlayerService(context, mockHasher.Object);

        var result = await service.GetPlayerByIdAsync(player.Id);

        Assert.NotNull(result);
        Assert.Equal(player.Id, result.Id);
        Assert.Equal(player.FirstName, result.FirstName);
        Assert.Equal(player.Surname, result.Surname);
        Assert.Equal(player.EmailAddress, result.EmailAddress);
        Assert.Equal(player.DateOfBirth, result.DateOfBirth);
    }

    [Fact]
    public async Task GetPlayerByIdAsync_WhenPlayerDoesNotExist_ShouldReturnNull()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();
        var service = new PlayerService(context, mockHasher.Object);

        var result = await service.GetPlayerByIdAsync(999);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task CreatePlayerAsync_ShouldHashPassword_AndSaveToDatabase()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();

        const string rawPassword = "SecretPassword!";
        const string hashedPassword = "HashedSecretPassword!";

        mockHasher.Setup(h => h.HashPassword(It.IsAny<Player>(), rawPassword)).Returns(hashedPassword);

        var service = new PlayerService(context, mockHasher.Object);
        var createDto = new CreatePlayerDto("Daniella", "Smith", new DateTime(2000, 05, 20), "Daniella.Smith@gmail.com", rawPassword);
        
        var result = await service.CreatePlayerAsync(createDto);
        
        Assert.NotNull(result);
        Assert.Equal(createDto.FirstName, result.FirstName);
        Assert.Equal(createDto.Surname, result.Surname);
        Assert.Equal(createDto.DateOfBirth, result.DateOfBirth);
        Assert.Equal(createDto.EmailAddress, result.EmailAddress);

        var savedPlayer = await context.Players.FirstOrDefaultAsync(p => p.Id == result.Id);
        Assert.NotNull(savedPlayer);
        Assert.Equal(createDto.FirstName, savedPlayer.FirstName);
        Assert.Equal(createDto.Surname, savedPlayer.Surname);
        Assert.Equal(createDto.DateOfBirth, savedPlayer.DateOfBirth);
        Assert.Equal(createDto.EmailAddress, savedPlayer.EmailAddress);
        Assert.Equal(hashedPassword, savedPlayer.PasswordHash);
    }

    [Fact]
    public async Task UpdatePlayerByIdAsync_WhenPlayerExists_ShouldUpdateOnlyProvidedFields()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();

        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 05, 20));
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new PlayerService(context, mockHasher.Object);
        var updateDto = new UpdatePlayerDto("Daniella-Updated", null, null, null);

        var result = await service.UpdatePlayerByIdAsync(player.Id, updateDto);

        Assert.NotNull(result);
        Assert.Equal(updateDto.FirstName, result.FirstName);
        Assert.Equal(player.Surname, result.Surname);

        var updatedPlayerInDb = await context.Players.FindAsync(player.Id);
        Assert.NotNull(updatedPlayerInDb);
        Assert.Equal(updateDto.FirstName, updatedPlayerInDb.FirstName);
        Assert.Equal(player.Surname, updatedPlayerInDb.Surname);
    }

    [Fact]
    public async Task UpdatePlayerByIdAsync_WhenPlayerDoesNotExist_ShouldReturnNull()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();
        var service = new PlayerService(context, mockHasher.Object);

        var updateDto = new UpdatePlayerDto("NewName", null, null, null);
        var result = await service.UpdatePlayerByIdAsync(999, updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePlayerAsync_WhenPlayerExists_ShouldRemoveFromDbAndReturnTrue()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();

        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 05, 20));
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new PlayerService(context, mockHasher.Object);

        var result = await service.DeletePlayerAsync(player.Id);

        Assert.True(result);

        var playerInDb = await context.Players.FindAsync(player.Id);
        Assert.Null(playerInDb);
    }

    [Fact]
    public async Task DeletePlayerAsync_WhenPlayerDoesNotExist_ShouldReturnFalse()
    {
        await using var context = GetInMemoryContext();
        var mockHasher = new Mock<IPasswordHasher<Player>>();
        var service = new PlayerService(context, mockHasher.Object);

        var result = await service.DeletePlayerAsync(999);

        Assert.False(result);
    }
}