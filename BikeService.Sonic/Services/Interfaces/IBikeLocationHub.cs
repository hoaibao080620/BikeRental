namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeLocationHub
{
    Task NotifyBikeLocationHasChanged(string? email);
}
