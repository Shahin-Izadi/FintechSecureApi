using Application.Common.Options;
using Application.DTOs.Auth;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace FintechSecureApi.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public void GenerateToken_Should_Create_Valid_Jwt_With_Correct_Claims()
    {

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

        var token = sut.GenerateToken(user).Result;
        
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

    [Fact]
    public void Password_Hashing_And_Verification_Works()
    {
        var options = Options.Create(new JwtSettings
        {
            Secret = "SuperSecretJwtKeyForFintechDemo2025!@#1234567890",
            Issuer = "FintechSecureApi",
            Audience = "FintechClients",
            ExpiryMinutes = 1440
        });

        var dbContext = CreateInMemoryDbContext();
        var sut = new AuthService(dbContext, options);

        const string plainPassword = "MySuperSecurePassword123!";

        var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);

 
        var isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashed);
        isValid.Should().BeTrue();

        var isInvalid = BCrypt.Net.BCrypt.Verify("wrong password", hashed);
        isInvalid.Should().BeFalse();

        var hashedAgain = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        hashed.Should().NotBe(hashedAgain);
    }
}