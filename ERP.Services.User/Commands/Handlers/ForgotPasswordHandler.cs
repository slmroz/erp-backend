using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class ForgotPasswordHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IPasswordManager _passwordManager;
    private readonly IEmailService _emailService;  // Inject your email sender
    private readonly IClock _clock;

    public ForgotPasswordHandler(ErpContext dbContext, IPasswordManager passwordManager,
        IEmailService emailService, IClock clock)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _emailService = emailService;
        _clock = clock;
    }

    public async Task HandleAsync(ForgotPasswordCommand command)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == command.Email).ConfigureAwait(false);
        if (user is null)
        {
            return;  // Silent fail for security - don't confirm email exists
        }

        // Generate secure token (e.g., GUID or JWT with expiry)
        var resetToken = Guid.NewGuid().ToString();  // In prod, use JWT or hashed token
        user.PasswordResetToken = resetToken;
        user.PasswordResetExpires = _clock.Current().AddHours(1);  // 1 hour expiry

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        // Send email with reset link: /api/auth/reset-password?token={resetToken}
        var resetUrl = $"https://teammate.pl/users/passwordReset?token={resetToken}";

        var model = new Dictionary<string, string>();
        model.Add("UserName", $"{user.FirstName} {user.LastName}");
        model.Add("ResetUrl", resetUrl);

        await _emailService.SendAsync(user.Email, "PasswordReset", model);
    }
}