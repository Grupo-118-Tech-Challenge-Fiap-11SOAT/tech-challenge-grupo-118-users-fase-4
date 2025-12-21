using Domain.Base.Exceptions;

namespace Domain.Employee.Exceptions;

public class PasswordNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "Password was null or empty.";
}
