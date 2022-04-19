namespace Core.Crosscutting;

public class M2MClientSettings
{
    public string? DiscoveryUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public bool UseHttps { get; set; }
}