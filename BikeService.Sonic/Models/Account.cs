﻿using Shared.Models;

namespace BikeService.Sonic.Models;

public class Account : BaseEntity
{
    public string Email { get; set; } = null!;
    public double Point { get; set; }
    public string ExternalId { get; set; } = null!;
}
