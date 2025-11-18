namespace Application.DTOs.Transactions;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}