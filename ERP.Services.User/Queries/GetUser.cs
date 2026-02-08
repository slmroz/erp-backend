using ERP.Services.Abstractions;
using ERP.Services.User.DTO;

namespace ERP.Services.User.Queries;
public class GetUser : IQuery<UserDto>
{
    public int UserId { get; set; }
}