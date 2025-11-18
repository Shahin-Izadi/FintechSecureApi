namespace Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FromAccountId { get; set; }
    public Account FromAccount { get; set; } = null!;

    public Guid? ToAccountId { get; set; }           // null = deposit / withdrawal
    public Account? ToAccount { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty; // Transfer, Deposit, Withdrawal
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}