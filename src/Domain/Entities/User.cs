using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; // User or Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Account> Accounts { get; set; } = new();
}