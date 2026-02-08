using ERP.Services.Abstractions;

namespace ERP.Services.User.Commands;
public record SignUp(string Email, string Password) : ICommand;