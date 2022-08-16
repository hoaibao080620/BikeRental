namespace BikeService.Sonic.Exceptions;

public class BikeNotFoundException : Exception
{
    public BikeNotFoundException(string id)
        : base($"Bike with code {id} not found!")
    {
    }
}
