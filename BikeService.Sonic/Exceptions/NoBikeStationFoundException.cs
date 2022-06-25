namespace BikeService.Sonic.Exceptions;

public class NoBikeStationFoundException : Exception
{
    public NoBikeStationFoundException()
        : base("Cannot find any bike stations")
    {
    }
}
