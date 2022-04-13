namespace Ordering.Application.Exceptions;

public class BadRequestException : ApplicationException
{
    public BadRequestException(string message)
        : base("Unprocessable request", message)
    {
    }
}