namespace Domain.Base.Exceptions;

public class InvalidCpfException : DomainException
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message => "CPF was invalid.";
}
