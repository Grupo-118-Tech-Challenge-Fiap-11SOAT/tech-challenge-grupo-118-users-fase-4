using Domain.Base.Exceptions;

namespace Domain.Employee.Exceptions;

public class CpfNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message indicating that the CPF was null or empty.
    /// </summary>
    public override string Message => "CPF was null or empty.";
}
