using Domain.Employee.Ports.Out;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Infra.Password;

public class PasswordManager : IPasswordManager
{
    private readonly IConfiguration _configuration;
    public PasswordManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>  
    /// Creates a hash for the specified password using HMACSHA512 and a secret key.  
    /// </summary>  
    /// <param name="password">The plain text password to hash.</param>  
    /// <param name="passwordHash">The resulting hashed password as a Base64-encoded string.</param>  
    public void CreatePasswordHash(string password, out string passwordHash)
    {
        var secretKey = Encoding.UTF8.GetBytes(_configuration["Security:Key"]);
        using var hmac = new HMACSHA512(secretKey);
        passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
