using AltaSemester.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Utils.Interface
{
    public interface IJwt
    {
        string GenerateJWT(User user, List<string> roles);
        string GenerateRefreshToken();
    }
}
