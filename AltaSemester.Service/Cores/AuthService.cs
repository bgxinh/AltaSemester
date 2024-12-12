﻿using AltaSemester.Data.DataAccess;
using AltaSemester.Service.Cores.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Data.Entities;
using AltaSemester.Data.Dtos;
using AltaJwtTokens;
using System.Security.Claims;
using AltaSemester.Service.Utils.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.File;
using Microsoft.AspNetCore.Hosting;
using MimeKit.Cryptography;
using System.Linq.Expressions;
using AltaSemester.Service.Utils.Mailer;
namespace AltaSemester.Service.Cores
{
    public class AuthService : IAuth
    {
        private AltaSemesterContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthService(AltaSemesterContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        private string GenerateAccessToken(User user)
        {
            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.UserRole)
                    };
            var jwtSettings = new JwtSettings
            {
                SecretKey = _configuration["Jwt:Key"],
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                ExpiryMinutes = 10
            };
            var jwt = new JwtTokenGenerator(jwtSettings);
            var token = jwt.GenerateToken(authClaims);
            return token;
        }
        public async Task<ModelResult> Login (string username, string password)
        {
            ModelResult _result = new ModelResult();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _result.Success = false;
                    _result.Message = "Missing username or password";
                    return _result;
                }
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if(user.IsFirstLogin == true)
                {
                    _result.Success = false;
                    _result.Message = "First login, please change your password";
                    return _result;
                }
                if (user == null)
                {
                    _result.Success = false;
                    _result.Message = "User does not exits";
                    return _result;
                }
                password = Encrypt.EncryptMd5(password);
                if (user.Password != password)
                {
                    _result.Success = false;
                    _result.Message = "Incorrect password. Please try again";
                    return _result;
                }
                //Cấp token
                var token = GenerateAccessToken(user);
                if (user.RefreshToken == null || user.ExpiredAt > DateTime.UtcNow.AddHours(7))
                {
                    var refreshToken = RefreshToken.GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.ExpiredAt = DateTime.UtcNow.AddHours(7).AddDays(7);
                    await _context.SaveChangesAsync();
                }
                LoginResponse _response = new LoginResponse
                {
                    Username = user.Username,
                    Fullname = user.FullName,
                    Email = user.Email,
                    Note = user.Note,
                    Token = token,
                    RefreshToken = user.RefreshToken,
                    Role = user.UserRole,
                    Avatar = user.Avatar,
                    IsFirstLogin = user.IsFirstLogin,
                };
                await _context.SaveChangesAsync();
                _result.Success = true;
                _result.Data = _response;
                _result.Message = "Login success";
                return _result;
            }
            catch (Exception ex)
            {
  
                _result.Success = false;
                _result.Message = ex.Message;
                return _result;
            }
        }
        public async Task<ModelResult> Logout(string username)
        {
            var _result = new ModelResult();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                if (user == null)
                {
                    _result.Success = false;
                    _result.Message = "User not found.";
                    return _result;
                }
                user.IsActive = false;
                await _context.SaveChangesAsync();

                _result.Success = true;
                _result.Message = "Logout successful.";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = $"An error occurred: {ex.Message}";
            }

            return _result;
        }

        public async Task<ModelResult> Refresh(string accessToken, string refreshToken)
        {
            var _result = new ModelResult();

            try
            {
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    _result.Message = "Missing access token or refresh token.";
                    _result.Success = false;
                    return _result;
                }
                var principal = RefreshToken.GetClaimsPrincipalToken(accessToken, _configuration);
                if (principal?.Identity?.Name is null)
                {
                    _result.Message = "Access token is not valid.";
                    _result.Success = false;
                    return _result;
                }
                var user = await _context.Users
                    .Where(x => x.Username == principal.Identity.Name)
                    .FirstOrDefaultAsync();
                if (user == null || !string.Equals(user.RefreshToken, refreshToken) || user.ExpiredAt < DateTime.UtcNow.AddHours(7))
                {
                    _result.Message = "Refresh token is invalid or expired.";
                    _result.Success = false;
                    return _result;
                }
                var newAccessToken = GenerateAccessToken(user);
                await _context.SaveChangesAsync();
                _result.Data = new RefreshDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = user.RefreshToken,
                };
                _result.Success = true;
                _result.Message = "Create new token success.";
            }
            catch (Exception ex)
            {
                _result.Message = $"An error occurred: {ex.Message}";
                _result.Success = false;
            }

            return _result;
        }

        public async Task<ModelResult> ResetPasswordFirstLogin(ResetPassword resetPassword)
        {
            var _result = new ModelResult();
            try
            {
                if (resetPassword == null)
                {
                    _result.Message = "Missing parameter";
                    _result.Success =false;
                    return _result;
                }
                var user = await _context.Users.Where(x => x.Username == resetPassword.Username).FirstOrDefaultAsync();
                if (user == null)
                {
                    _result.Message = "User not found";
                    _result.Success=false;
                    return _result;
                }
                if (user.Password != Encrypt.EncryptMd5(resetPassword.Password))
                {
                    _result.Message = "Password not correct";
                    _result.Success = false;
                    return _result;
                }
                user.Password = Encrypt.EncryptMd5(resetPassword.NewPassword);
                user.IsFirstLogin = false;
                await _context.SaveChangesAsync();
                _result.Message = "Change password success";
                _result.Success = true;
            }
            catch (Exception ex) 
            {
                _result.Message = ex.ToString();
                _result.Success=false;
            }
            return _result;
        }

        public async Task<ModelResult> ResetPassword(ResetPassword resetPassword)
        {
            var _result = new ModelResult();
            try
            {
                if (resetPassword == null)
                {
                    _result.Message = "Missing parameter";
                    _result.Success = false;
                    return _result;
                }
                var user = await _context.Users.Where(x => x.Username == resetPassword.Username).FirstOrDefaultAsync();
                if (user == null)
                {
                    _result.Message = "User not found";
                    _result.Success = false;
                    return _result;
                }
                if (user.PasswordReset != resetPassword.Password)
                {
                    _result.Message = "Password not correct";
                    _result.Success = false;
                    return _result;
                }
                user.Password = Encrypt.EncryptMd5(resetPassword.NewPassword);
                user.PasswordReset = null;
                await _context.SaveChangesAsync();
                _result.Message = "Change password success";
                _result.Success = true;
            }
            catch(Exception ex)
            {
                _result.Message = ex.ToString();
                _result.Success=false;
            }
            return _result;
        }

        public async Task<ModelResult> ForgotPassword(string email)
        {
            var _result = new ModelResult();
            try
            {
                var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
                if (user == null) 
                {
                    _result.Message = "Email does not exist";
                    _result.Success = false;
                    return _result;
                }
                var passwordReset = RefreshToken.GenerateRefreshToken();
                user.PasswordReset = passwordReset;
                await _context.SaveChangesAsync();
                _result = await Mail.SendMailResetPassword(email, passwordReset, _configuration);
            }
            catch (Exception ex) 
            {
                _result.Message = ex.ToString();
                _result.Success=false;
            }
            return _result;
        }
    }
}
