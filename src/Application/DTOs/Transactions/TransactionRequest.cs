using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Transactions;

public class CreateTransactionRequest
{
    [Required]
    public Guid FromAccountId { get; set; }

    public Guid? ToAccountId { get; set; } // null = deposit/withdrawal

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;
}