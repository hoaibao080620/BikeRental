using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserService.Dtos;
using UserService.Extensions;

namespace UserService.Clients;

public class OktaClient : IOktaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public OktaClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.AddOktaAuthorizationHeader();
        _baseUrl = configuration["Okta:Domain"];
    }
    
    public async Task<string?> AddUserToOkta(OktaUserInsertDto oktaUserInsertDto)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(new
            {
                profile = new
                {
                    firstName = oktaUserInsertDto.FirstName,
                    lastName = oktaUserInsertDto.LastName,
                    email = oktaUserInsertDto.Email,
                    login = oktaUserInsertDto.Email,
                    mobilePhone =  oktaUserInsertDto.PhoneNumber
                },
                credentials = new
                {
                    password = new
                    {
                        value = oktaUserInsertDto.Password
                    }
                },
                groupIds = new List<string>
                {
                    oktaUserInsertDto.GroupId
                }
            }), 
            Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/users?activate=true", content);
        var responseBody = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
        return responseBody?["id"]?.ToString();
    }

    public async Task DeleteOktaUser(string? oktaUserId)
    {
        await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/api/v1/users/{oktaUserId}/lifecycle/deactivate", 
            string.Empty);

        await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/users/{oktaUserId}");
    }
}