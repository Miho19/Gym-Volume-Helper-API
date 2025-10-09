using System.Text;
using dotenv.net;
using Newtonsoft.Json;

namespace gymapi.src.AuthManagement.Auth0;

public class Auth0Management : IAuthManagement
{

    private readonly ILogger<Auth0Management> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly string Auth0ApiBaseURL = "/api/v2/";

    private string Auth0ManagementAccessToken = string.Empty;

    public Auth0Management(ILogger<Auth0Management> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public bool IsInitilised { get; private set; }



    public async Task Initialise()
    {
        try
        {

            if (IsInitilised) return;


            var httpClient = getInitialiseHttpClient();
            var initaliseRequestMessage = getInitialiseHttpRequestMessage();

            var httpResponseMessage = await httpClient.SendAsync(initaliseRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                IsInitilised = false;
                throw new Exception("Failed to initialise Auth0 Management System");
            }

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Auth0ManagementInitialiseResponseBody>(responseString);

            if (responseObject is null)
            {
                IsInitilised = false;
                throw new Exception("Failed to initialise Auth0 Management System");
            }

            if (String.IsNullOrEmpty(responseObject.access_token))
            {
                IsInitilised = false;
                throw new Exception("Failed to initialise Auth0 Management System");
            }

            this.Auth0ManagementAccessToken = responseObject.access_token;
            IsInitilised = true;

        }
        catch (Exception e)
        {
            _logger.LogInformation($"Auth0 Management System error:\n {e.Message}");
        }

    }


    // Doing this for now for extension into using typed clients
    private HttpClient getInitialiseHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }

    private HttpRequestMessage getInitialiseHttpRequestMessage()
    {
        DotEnv.Load();
        var Auth0URL = System.Environment.GetEnvironmentVariable("Auth0URL");
        var Auth0ClientID = System.Environment.GetEnvironmentVariable("Auth0ClientID");
        var Auth0ClientSecret = System.Environment.GetEnvironmentVariable("Auth0ClientSecret");

        var headers = new
        {
            client_id = Auth0ClientID,
            client_secret = Auth0ClientSecret,
            audience = $"{Auth0URL}{Auth0ApiBaseURL}",
            grant_type = "client_credentials",
        };

        var stringHeaders = JsonConvert.SerializeObject(headers);
        var requestContent = new StringContent(stringHeaders, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{Auth0URL}/oauth/token");
        request.Content = requestContent;

        return request;
    }

    private HttpRequestMessage GetFetchUserHttpRequestMessage(string userID)
    {
        DotEnv.Load();
        var Auth0URL = System.Environment.GetEnvironmentVariable("Auth0URL");

        if (String.IsNullOrEmpty(userID)) throw new Exception($"{nameof(GetFetchUserHttpRequestMessage)}: requires userID");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{Auth0URL}{Auth0ApiBaseURL}{userID}");
        return request;
    }

    private HttpClient getFetchUserHttpClient()
    {
        if (!IsInitilised) throw new Exception($"{nameof(getFetchUserHttpClient)}\nAuth0 Management System must be initialised");
        var httpClient = getInitialiseHttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Auth0ManagementAccessToken);
        return httpClient;
    }

    public async Task<IAuthManagementFetchUserResponse> FetchUser(string userID)
    {
        try
        {
            if (String.IsNullOrEmpty(userID)) throw new Exception($"{nameof(FetchUser)} requires a valid userID");
            if (!IsInitilised) await Initialise();

            var httpClient = getFetchUserHttpClient();
            var fetchUserRequestMessage = GetFetchUserHttpRequestMessage(userID);

            var fetchUserResponseMessage = await httpClient.SendAsync(fetchUserRequestMessage);

            if (!fetchUserResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch user");
            }

            var responseString = await fetchUserResponseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Auth0ManagementSystemFetchUserResponse>(responseString);

            if (responseObject is null)
            {
                throw new Exception("Failed to convert fetch user response string to json object");
            }

            return responseObject;
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Auth0 Management System Fetch User error:\n {e.Message}");
            throw new Exception($"Auth0 Management System Fetch User error:\n {e.Message}");
        }

    }
}


public class Auth0ManagementInitialiseResponseBody
{
    public string? access_token;
}
