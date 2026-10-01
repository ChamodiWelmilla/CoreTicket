namespace SupportTicketSystem.Tests.UnitTests;

using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketSystem.Application.Common.Interfaces;
using SupportTicketSystem.Application.Dtos;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Infrastructure.Persistence;
using Xunit;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnToken()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        var hasherMock = new Mock<IPasswordHasher>();
        var tokenGenMock = new Mock<IJwtTokenGenerator>();

        hasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed_pass");
        tokenGenMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("mocked_jwt_token");

        var service = new AuthService(dbContext, hasherMock.Object, tokenGenMock.Object);
        var request = new RegisterRequest("Test User", "test@example.com", "Password123", UserRole.Customer);

        var result = await service.RegisterAsync(request);

        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("mocked_jwt_token", result.Token);
        Assert.Single(dbContext.Users);
    }
}
