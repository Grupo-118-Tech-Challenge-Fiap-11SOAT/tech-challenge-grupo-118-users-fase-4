using Domain.Base.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Employee.Exceptions;

[ExcludeFromCodeCoverage]
public class BirthDayMinValueException : DomainException
{
    /// <summary>
    /// Gets the error message indicating that the birth date is invalid.
    /// </summary>
    public override string Message => "BirthDay was less than 1900-01-01.";
}
