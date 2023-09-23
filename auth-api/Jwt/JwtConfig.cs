namespace auth_api.Jwt;

public class JwtConfig
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Key { get; set; } = "";
    
    public int LifetimeSeconds { get; set; }
}