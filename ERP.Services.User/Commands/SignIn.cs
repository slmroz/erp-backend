using ERP.Services.Abstractions;

namespace ERP.Services.User.Commands;
public record SignIn(string Email, string Password) : ICommand;