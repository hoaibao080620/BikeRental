namespace UserService.Exceptions;

public class AccountNotfoundException : Exception
{
    public AccountNotfoundException(string message)
        : base(message)
    {
    }
}
