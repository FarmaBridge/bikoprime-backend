namespace BikoPrime.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; set; }

    public DomainException(string message, string code = "INTERNAL_ERROR") 
        : base(message)
    {
        Code = code;
    }
}
