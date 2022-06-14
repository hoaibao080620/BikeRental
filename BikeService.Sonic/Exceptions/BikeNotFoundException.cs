namespace BikeService.Sonic.Exceptions;

public class BikeNotFoundException : Exception
{
    public BikeNotFoundException(int id)
        : base($"Bike with id {id} not found!")
    {
    }
}