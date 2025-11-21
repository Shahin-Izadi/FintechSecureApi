using Application.Common.Options;
using Application.DTOs.Accounts;
using Application.DTOs.Auth;
using FintechSecureApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Xunit;

namespace FintechSecureApi.Tests.Integration;

public class FullFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public FullFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Use InMemory DB
                services.Remove(services.SingleOrDefault(d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<Infrastructure.Persistence.AppDbContext>)));
                services.AddDbContext<Infrastructure.Persistence.AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                // Force the same JWT secret as in appsettings.json so the token is valid
                services.Configure<JwtSettings>(options =>
                {
                    options.Secret = "SuperSecretJwtKeyForFintechDemo2025!@#1234567890";
                    options.Issuer = "FintechSecureApi";
                    options.Audience = "FintechClients";
                    options.ExpiryMinutes = 1440;
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Full_Banking_Flow_Works()
    {
        // 1. Register
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = "test@test.com",
            Password = "Test123!",
            FullName = "Test User"
        });
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // 2. Set token
        _client.DefaultRequestHeaders.Authorization = new("Bearer", auth!.Token);

        // 3. Create two accounts
        var acc1 = await CreateAccount("AMD", 100000);
        var acc2 = await CreateAccount("AMD", 0);

        // 4. Transfer 25000 AMD
        var transferResponse = await _client.PostAsJsonAsync("/api/transactions", new
        {
            fromAccountId = acc1.Id,
            toAccountId = acc2.Id,
            amount = 25000m,
            description = "Test transfer"
        });
        transferResponse.EnsureSuccessStatusCode();

        // 5. Check balances
        var accounts = await _client.GetFromJsonAsync<List<AccountResponse>>("/api/accounts");
        var from = accounts!.First(a => a.Id == acc1.Id);
        var to = accounts!.First(a => a.Id == acc2.Id);

        Assert.Equal(75000m, from.Balance);
        Assert.Equal(25000m, to.Balance);
    }

    private async Task<AccountResponse> CreateAccount(string currency, decimal deposit)
    {
        var response = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest
        {
            Currency = currency,
            InitialDeposit = deposit
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }
}