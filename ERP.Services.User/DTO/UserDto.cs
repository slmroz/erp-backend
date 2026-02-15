namespace ERP.Services.User.DTO;
public class UserDto
{
    public int Id { get; set; }
    public int Role { get; set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string Email { get; set; }

    public UserDto(Model.Model.User user)
    {
        Id = user.Id;
        Email = user.Email;
        Role = user.Role;
        FirstName = user.FirstName;
        LastName = user.LastName;
    }
}
