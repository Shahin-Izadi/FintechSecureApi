using Application.DTOs.Accounts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AccountsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId => Guid.Parse(
    User.FindFirst("sub")?.Value ??
    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
    throw new InvalidOperationException("User ID not found in token"));

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request)
    {
        var account = new Account
        {
            UserId = CurrentUserId,
            AccountNumber = "AM" + Random.Shared.Next(10000000, 99999999).ToString(),
            Currency = request.Currency.ToUpperInvariant(),
            Balance = request.InitialDeposit
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        return Ok(new AccountResponse
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            Currency = account.Currency,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountResponse>>> GetMyAccounts()
    {
        var accounts = await _db.Accounts
            .Where(a => a.UserId == CurrentUserId)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Currency = a.Currency,
                Balance = a.Balance,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(accounts);
    }
}