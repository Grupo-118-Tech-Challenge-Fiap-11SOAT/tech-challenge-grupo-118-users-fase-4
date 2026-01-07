using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidEmailException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "Email was invalid.";
}
