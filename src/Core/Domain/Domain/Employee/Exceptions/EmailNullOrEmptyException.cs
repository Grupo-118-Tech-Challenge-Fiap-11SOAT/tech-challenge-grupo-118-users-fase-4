using Domain.Base.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Employee.Exceptions;

[ExcludeFromCodeCoverage]
public class EmailNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "Email was null or empty.";
}
