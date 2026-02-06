using ERP.Model.Model;
using ERP.Services.Abstractions.Security;
using Microsoft.AspNetCore.Identity;

namespace ERP.Infrastructure.Security;
internal sealed class PasswordManager : IPasswordManager
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordManager(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Secure(string password) => _passwordHasher.HashPassword(default, password);

    public bool Validate(string password, string securedPassword)
        => _passwordHasher.VerifyHashedPassword(default, securedPassword, password) ==
           PasswordVerificationResult.Success;
}