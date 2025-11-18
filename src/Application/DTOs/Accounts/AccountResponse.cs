namespace Application.DTOs.Accounts;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}