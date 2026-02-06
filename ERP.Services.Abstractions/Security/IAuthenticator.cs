using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Abstractions.Security;
public interface IAuthenticator
{
    JwtDto CreateToken(int userId, string role, string name);
}