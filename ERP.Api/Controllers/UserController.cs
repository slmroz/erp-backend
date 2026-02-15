using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands;
using ERP.Services.User.DTO;
using ERP.Services.User.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IQueryHandler<GetUsersQuery, IEnumerable<UserDto>> _getUsersHandler;
    private readonly IQueryHandler<GetUserQuery, UserDto> _getUserHandler;
    private readonly ICommandHandler<SignUpCommand> _signUpHandler;
    private readonly ICommandHandler<SignInCommand> _signInHandler;
    private readonly ICommandHandler<UpdateUserCommand> _updateUserHandler;
    private readonly ICommandHandler<ChangePasswordCommand> _changePasswordHandler;
    private readonly ICommandHandler<ForgotPasswordCommand> _forgotPasswordHandler;
    private readonly ICommandHandler<ResetPasswordCommand> _resetPasswordHandler;
    private readonly ITokenStorage _tokenStorage;

    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public UserController(ICommandHandler<SignUpCommand> signUpHandler,
        ICommandHandler<SignInCommand> signInHandler,
        ICommandHandler<UpdateUserCommand> updateUserHandler,
        ICommandHandler<ChangePasswordCommand> changePasswordHandler,
        ICommandHandler<ForgotPasswordCommand> forgotPasswordHandler,
        ICommandHandler<ResetPasswordCommand> resetPasswordHandler,
        IQueryHandler<GetUsersQuery, IEnumerable<UserDto>> getUsersHandler,
        IQueryHandler<GetUserQuery, UserDto> getUserHandler,
        ITokenStorage tokenStorage,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _signUpHandler = signUpHandler;
        _signInHandler = signInHandler;
        _updateUserHandler = updateUserHandler;
        _changePasswordHandler = changePasswordHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resetPasswordHandler = resetPasswordHandler;
        _getUsersHandler = getUsersHandler;
        _getUserHandler = getUserHandler;
        _tokenStorage = tokenStorage;

        _configuration = configuration;
        _emailService = emailService;
    }

    //[Authorize(Policy = "is-admin")]
    [Authorize(Roles = "Admin")]
    [HttpGet("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Get(int userId)
    {
        var user = await _getUserHandler.HandleAsync(new GetUserQuery { UserId = userId });
        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Get()
    {
        var userId = int.Parse(User.Identity?.Name);

        if (userId == 0)
        {
            return NotFound();
        }

        var user = await _getUserHandler.HandleAsync(new GetUserQuery { UserId = userId }).ConfigureAwait(false);

        return user;
    }

    [HttpGet]
    [SwaggerOperation("Get list of all the users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    //[Authorize(Policy = "is-admin")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] GetUsersQuery query)
        => Ok(await _getUsersHandler.HandleAsync(query));

    [HttpPost]
    [SwaggerOperation("Create the user account")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(SignUpCommand command)
    {
        await _signUpHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { command.Email }, null);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation("Update the user account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put(int id, UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _updateUserHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPost("sign-in")]
    [SwaggerOperation("Sign in the user and return the JSON Web Token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JwtDto>> Post(SignInCommand command)
    {
        await _signInHandler.HandleAsync(command);
        var jwt = _tokenStorage.Get();
        //_logger.LogInformation("Token {token} assigned in successfully.", jwt);
        return jwt;
    }

    // Controller method
    [HttpPut("change-password")]
    [SwaggerOperation("Change the user password")]
    [Authorize]  // Assuming JWT or similar auth to identify user
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangePassword(ChangePasswordCommand command)
    {
        await _changePasswordHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [SwaggerOperation("Request password reset via email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        await _forgotPasswordHandler.HandleAsync(command);
        return Ok();  // Don't reveal if email exists for security
    }

    [HttpPost("reset-password")]
    [SwaggerOperation("Reset password using token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(ResetPasswordCommand command)
    {
        await _resetPasswordHandler.HandleAsync(command);
        return NoContent();
    }

    //[HttpGet("test-email-config")]
    //public async Task<IActionResult> TestConfig([FromServices] IEmailService email)
    //{
    //    var password = _configuration["EmailSettings:Password"];
    //    var exception = "All fine";
    //    try
    //    {
    //        await _emailService.SendAsync("slmroz@wp.pl", "PasswordReset", new { UserName = "Slawek", ResetUrl = "https://teammate.pl" });
    //    }
    //    catch (Exception ex)
    //    {
    //        exception = ex.Message;
    //    }
    //    return Ok($"Email password configured: {password}. Exception: {exception}");
    //}
}
