using System.Diagnostics.CodeAnalysis;

namespace Domain.Base.Entities;

[ExcludeFromCodeCoverage]
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
