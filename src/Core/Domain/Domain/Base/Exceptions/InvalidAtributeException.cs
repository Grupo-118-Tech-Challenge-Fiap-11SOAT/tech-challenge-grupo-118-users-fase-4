using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidAtributeException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAtributeException"/> class.
    /// </summary>
    public InvalidAtributeException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAtributeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public InvalidAtributeException(string atributeName)
        : base($"The attribute {atributeName} is invalid.")
    {
    }
}
