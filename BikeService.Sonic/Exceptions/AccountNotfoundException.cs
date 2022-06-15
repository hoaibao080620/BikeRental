namespace BikeService.Sonic.Exceptions;

public class AccountNotfoundException : Exception
{
    public AccountNotfoundException(string message)
        : base(message)
    {
    }
}
