namespace gateway_api.Models.Auth;

public static class Auth
{
    public record Request(string Login, string Password);
}