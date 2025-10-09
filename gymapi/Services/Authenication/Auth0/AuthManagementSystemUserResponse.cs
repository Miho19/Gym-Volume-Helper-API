namespace gymapi.src.AuthManagement.Auth0;

public class Auth0ManagementSystemFetchUserResponse : IAuthManagementFetchUserResponse
{
    public string? DisplayName { get; set; }
    public string? UserID { get; set; }
    public List<Identities>? Identities { get; set; }
    public string? Image { get; set; }
    public string? AccessToken { get; set; }
}

public class Identities
{
    public string? access_token;
}