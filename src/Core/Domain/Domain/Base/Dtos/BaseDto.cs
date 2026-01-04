using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Dtos;

[ExcludeFromCodeCoverage]
public class BaseDto
{
    public BaseDto()
    {
        Error = false;
        ErrorMessage = string.Empty;
    }

    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string ErrorMessage { get; set; }
    public bool Error { get; set; } = false;
}
