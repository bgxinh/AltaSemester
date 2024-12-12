using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IAuth
    {
        public Task<ModelResult> Login (string username, string password);
        public Task<ModelResult> Refresh(string accessToken, string refreshToken);
        public Task<ModelResult> Logout(string Username);
        public Task<ModelResult> ResetPasswordFirstLogin(ResetPassword resetPassword);
        public Task<ModelResult> ResetPassword(ResetPassword resetPassword);
        public Task<ModelResult> ForgotPassword(string email);
    }
}
