namespace ERP.Services.User.DTO;
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }

    public UserDto(Model.Model.User user)
    {
        Id = user.Id;
        Email = user.Email;
    }
}
