using Domain.Base.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Employee.Exceptions;

[ExcludeFromCodeCoverage]
public class PasswordNullOrEmptyException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "Password was null or empty.";
}
