﻿namespace BikeService.Sonic.Dtos;

public class MarkReportAsResolveDto
{
    public int BikeReportId { get; set; }
    public string Status { get; set; } = null!;
    public int AssignToId { get; set; }
}
