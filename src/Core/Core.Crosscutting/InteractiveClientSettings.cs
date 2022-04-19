namespace Core.Crosscutting;

public class InteractiveClientSettings
{
    public string? RedirectUri { get; set; }
    public string? FrontChannelLogoutUri { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Authority { get; set; }
    public List<string>? Scopes { get; set; }
}