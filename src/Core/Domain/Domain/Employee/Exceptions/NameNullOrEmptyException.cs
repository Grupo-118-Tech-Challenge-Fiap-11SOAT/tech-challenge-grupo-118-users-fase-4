using Domain.Base.Exceptions;

namespace Domain.Employee.Exceptions;

public class NameNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message indicating that the name was null or empty.
    /// </summary>
    public override string Message => "Name was null or empty.";
}
