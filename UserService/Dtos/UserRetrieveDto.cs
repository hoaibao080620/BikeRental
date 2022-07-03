﻿namespace UserService.Dtos;

public class UserRetrieveDto
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
