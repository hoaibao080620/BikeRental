using UserService.Dtos;

namespace UserService.Clients;

public interface IOktaClient
{
    public Task<string?> AddUserToOkta(OktaUserInsertDto oktaUserInsertDto);
    public Task DeleteOktaUser(string? oktaUserId);
}