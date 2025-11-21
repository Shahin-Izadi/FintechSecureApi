using Application.DTOs.Transactions;
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
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId => Guid.Parse(
        User.FindFirst("sub")?.Value ??
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
        throw new InvalidOperationException("User ID not found in token"));

    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create(CreateTransactionRequest request)
    {
        // Load fromAccount with lock
        var fromAccount = await _db.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.FromAccountId && a.UserId == CurrentUserId);

        if (fromAccount == null)
            return NotFound("Source account not found or doesn't belong to you");

        if (fromAccount.Balance < request.Amount)
            return BadRequest("Insufficient funds");

        Account? toAccount = null;
        string type = request.Amount > 0 ? "Deposit" : "Withdrawal";

        if (request.ToAccountId.HasValue)
        {
            toAccount = await _db.Accounts
                .FirstOrDefaultAsync(a => a.Id == request.ToAccountId.Value);

            if (toAccount == null)
                return BadRequest("Destination account does not exist"); // ← now 400 instead of 404

            type = "Transfer";
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            fromAccount.Balance -= request.Amount;
            if (toAccount != null)
                toAccount.Balance += request.Amount;

            var transaction = new Transaction
            {
                FromAccountId = fromAccount.Id,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
                Type = type,
                Description = request.Description ?? string.Empty
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new TransactionResponse
            {
                Id = transaction.Id,
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt
            });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<TransactionResponse>>> GetMyTransactions()
    {
        var transactions = await _db.Transactions
            .Where(t => t.FromAccount.UserId == CurrentUserId ||
                       (t.ToAccountId != null && t.ToAccount!.UserId == CurrentUserId))
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                FromAccountId = t.FromAccountId,
                ToAccountId = t.ToAccountId,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return Ok(transactions);
    }
}