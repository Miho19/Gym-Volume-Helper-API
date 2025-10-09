namespace gymapi.src.AuthManagement;

public interface IAuthManagement
{
    bool IsInitilised { get; }
    Task Initialise();
    Task<IAuthManagementFetchUserResponse> FetchUser(string userID);

}


public interface IAuthManagementFetchUserResponse
{
    
}