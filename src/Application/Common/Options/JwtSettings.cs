namespace Application.Common.Options;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "FintechSecureApi";
    public string Audience { get; set; } = "FintechClients";
    public int ExpiryMinutes { get; set; } = 1440; // 24 hours
}