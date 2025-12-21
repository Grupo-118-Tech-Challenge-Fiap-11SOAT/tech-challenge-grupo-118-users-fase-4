namespace Domain.Employee.Ports.Out;

public interface IPasswordManager
{
    /// <summary>
    /// Creates a hash for the specified password.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <param name="passwordHash">The resulting hashed password.</param>
    void CreatePasswordHash(string password, out string passwordHash);
}
