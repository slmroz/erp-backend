using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Abstractions.Security;
public interface IPasswordManager
{
    string Secure(string password);
    bool Validate(string password, string securedPassword);
}