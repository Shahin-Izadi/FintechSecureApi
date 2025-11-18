using System.Transactions;

namespace Domain.Entities;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string AccountNumber { get; set; } = string.Empty; // e.g. AM0012345678
    public string Currency { get; set; } = "AMD";
    public decimal Balance { get; set; } = 0m;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Transaction> Transactions { get; set; } = new();
}