using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class ChangePasswordHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly ErpContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;  // To get current user

    public ChangePasswordHandler(ErpContext dbContext, IPasswordManager passwordManager,
        IClock clock, IHttpContextAccessor httpContextAccessor)
    {
        _passwordManager = passwordManager;
        _clock = clock;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(ChangePasswordCommand command)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == int.Parse(userIdClaim)).ConfigureAwait(false);
        if (user is null)
        {
            throw new Exception("User not found");
        }

        // Verify current password (assuming Password is the hashed version)
        if (!_passwordManager.Validate(command.CurrentPassword, user.Password))
        {
            throw new Exception("Current password is incorrect");
        }

        // Check if new password is same as current
        var newSecuredPassword = _passwordManager.Secure(command.NewPassword);
        if (newSecuredPassword == user.Password)
        {
            throw new Exception("New password cannot be the same as current");
        }

        user.Password = newSecuredPassword;
        user.LastUpdatedAt = _clock.Current();  

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}