using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Accounts;

public class CreateAccountRequest
{
    [Required]
    public string Currency { get; set; } = "AMD"; // or like USD, EUR, etc.

    public decimal InitialDeposit { get; set; } = 0m;
}