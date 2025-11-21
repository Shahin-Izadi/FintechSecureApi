using Application.Common.Options;
using Application.DTOs.Auth;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace FintechSecureApi.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public void GenerateToken_Should_Create_Valid_Jwt_With_Correct_Claims()
    {
        // Arrange
        var options = Options.Create(new JwtSettings
        {
            Secret = "SuperSecretJwtKeyForFintechDemo2025!@#1234567890",
            Issuer = "FintechSecureApi",
            Audience = "FintechClients",
            ExpiryMinutes = 1440
        });

        var dbContext = CreateInMemoryDbContext();
        var sut = new AuthService(dbContext, options);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FullName = "Test User",
            Role = "Admin"
        };

        // Act
        var token = sut.GenerateToken(user).Result;
        
        // Assert
        token.Should().NotBeNull();
        token.Token.Should().NotBeEmpty();
        token.Email.Should().Be("test@test.com");
        token.FullName.Should().Be("Test User");
        token.Role.Should().Be("Admin");
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}