using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserService.Dtos;
using UserService.Dtos.OktaClient;
using UserService.Extensions;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

    public async Task<List<OktaUserResponse>> GetOktaUserByGroup(string groupId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/groups/{groupId}/users");
        var body = await response.Content.ReadAsStringAsync();

        var oktaUsers = JsonConvert.DeserializeObject<List<OktaUserResponse>>(body);

        return oktaUsers ?? new List<OktaUserResponse>();
    }

    public async Task<List<OktaRoleResponse>> GetOktaGroups()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/groups?filter=type eq \"OKTA_GROUP\"");
        var body = await response.Content.ReadAsStringAsync();

        var oktaUsers = JsonConvert.DeserializeObject<List<OktaRoleResponse>>(body);

        return oktaUsers ?? new List<OktaRoleResponse>();
    }

    public async Task DeleteOktaUser(string? oktaUserId)
    {
        await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/api/v1/users/{oktaUserId}/lifecycle/deactivate", 
            string.Empty);

        await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/users/{oktaUserId}");
    }

    public async Task UpdateOktaUserRole(string oldGroupId, string newGroupId, string oktaUserId)
    {
        await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/groups/{oldGroupId}/users/{oktaUserId}");
        await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}/api/v1/groups/{newGroupId}/users/{oktaUserId}", 
            string.Empty);
    }

    public async Task UpdateOktaUserPassword(string oktaUserId, string password)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(new
            {
                credentials = new
                {
                    password = new
                    {
                        value = password
                    }
                }
            }), 
            Encoding.UTF8, "application/json");

        await _httpClient.PostAsync($"{_baseUrl}/api/v1/users/{oktaUserId}", content);
    }

    public async Task DeactivateOktaUser(string oktaUserId)
    {
        await _httpClient.PostAsync($"{_baseUrl}/api/v1/users/{oktaUserId}/lifecycle/suspend", null);
    }

    public async Task ActivateOktaUser(string oktaUserId)
    {
        await _httpClient.PostAsync($"{_baseUrl}/api/v1/users/{oktaUserId}/lifecycle/unsuspend", null);
    }

    public async Task<bool> IsPasswordValid(string username, string password)
    {
        var result = await _httpClient
            .PostAsJsonAsync($"{_baseUrl}/api/v1/authn", new
            {
                username, password
            });

        return result.StatusCode == HttpStatusCode.OK;
    }
}
