using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers()
        => Ok(await _db.Users.Select(u => new
        {
            u.Id,
            u.Email,
            u.FullName,
            u.Role,
            u.CreatedAt,
            Accounts = u.Accounts.Select(a => new { a.Id, a.AccountNumber, a.Currency, a.Balance })
        }).ToListAsync());

    [HttpGet("accounts")]
    public async Task<ActionResult> GetAllAccounts()
        => Ok(await _db.Accounts
            .Include(a => a.User)
            .Select(a => new
            {
                a.Id,
                a.AccountNumber,
                a.Currency,
                a.Balance,
                a.CreatedAt,
                Owner = a.User.Email
            })
            .ToListAsync());

    [HttpGet("transactions")]
    public async Task<ActionResult> GetAllTransactions()
        => Ok(await _db.Transactions
            .Include(t => t.FromAccount).ThenInclude(a => a!.User)
            .Include(t => t.ToAccount).ThenInclude(a => a!.User)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Type,
                t.Amount,
                t.Description,
                t.CreatedAt,
                From = t.FromAccount != null ? t.FromAccount.AccountNumber + " (" + t.FromAccount.User.Email + ")" : null,
                To = t.ToAccount != null ? t.ToAccount.AccountNumber + " (" + t.ToAccount.User.Email + ")" : null
            })
            .ToListAsync());
}