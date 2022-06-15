namespace BikeService.Sonic.Exceptions;

public class UserHasNotRentAnyBikeException : Exception
{
    public UserHasNotRentAnyBikeException(string email)
        : base($"User with email {email} currently does not rent any bike")
    {
    }
}
