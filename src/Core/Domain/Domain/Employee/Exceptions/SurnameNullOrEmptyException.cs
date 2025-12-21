using Domain.Base.Exceptions;

namespace Domain.Employee.Exceptions;

public class SurnameNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "Surname was null or empty.";
}
