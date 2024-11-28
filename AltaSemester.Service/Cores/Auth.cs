using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.AuthDtos;
using AltaSemester.Service.Cores.Interface;
using AltaSemester.Service.Utils.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Data.Entities;
using System.Net.WebSockets;
using AltaSemester.Data.Enums;
namespace AltaSemester.Service.Cores
{
    public class Auth : IAuth
    {
        private AltaSemesterContext _context;
        private ModelResult _result;
        private IMapper _mapper;
        private IEmailService _emailService;
        private IJwt _jwt;
        private readonly IConfiguration _configuration;
        public Auth(AltaSemesterContext context, IMapper mapper, IEmailService emailService, IJwt jwt, IConfiguration configuration)
        {
            _context = context;
            _result = new ModelResult();
            _mapper = mapper;
            _emailService = emailService;
            _jwt = jwt;
            _configuration = configuration;
        }
        private string HashEmail(string email)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(email);
                var hashEmail = sha256.ComputeHash(bytes);
                var builder = new StringBuilder();
                foreach (var item in hashEmail)
                {
                    builder.Append(item.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private ClaimsPrincipal? GetClaimsPrincipalToken(string? token)
        {
            var validation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }
        private bool VerifyPassword(string password, string userPassword)
        {
            return password.Equals(userPassword);
        }
        public Task<ModelResult> EmailConfirm(string hashedEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<ModelResult> Login(LoginDto loginDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                    {
                        _result.IsSuccess = false;
                        _result.Message = "Missing parameter";
                    }
                    var user = await _context.Users.Where(u => u.Username == loginDto.Username).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        _result.IsSuccess = false;
                        _result.Message = "User does not exist";
                        return _result;
                    }
                    //if (user.IsEmailConfirmed != true)
                    //{
                    //    _result.IsSuccess = false;
                    //    _result.Message = "Active account in mail to login";
                    //    return _result;
                    //}
                    string hashPass = Encrypt.EncryptMd5(loginDto.Password);
                    var checkPassword = VerifyPassword(hashPass, user.Password);
                    if (!checkPassword)
                    {
                        _result.IsSuccess = false;
                        _result.Message = "Incorrect password. Please try again";
                        return _result;
                    }
                    var roles = await _context.UserRoles
                        .Where(x => x.UserId == user.Id)
                        .Select(x => x.Role.RoleName)
                        .ToListAsync();
                    var token = _jwt.GenerateJWT(user, roles);
                    if (user.RefreshToken == null || user.ExpiredAt < DateTime.UtcNow) 
                    {
                        user.RefreshToken = _jwt.GenerateRefreshToken();
                        user.ExpiredAt = DateTime.UtcNow.AddDays(7);
                    }
                    await _context.SaveChangesAsync();
                    LoginResponse loginResponse = new LoginResponse
                    {
                        RefreshToken = user.RefreshToken,
                        Username = user.Username,
                        Email = user.Email,
                        Token = token,
                    };
                    _result.IsSuccess = true;
                    _result.Data = loginResponse;
                    _result.Message = "Login success";
                    await transaction.CommitAsync();
                    return _result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _result.IsSuccess = false;
                    _result.Message = ex.Message;
                    return _result;
                }
            }
        }

        public async Task<ModelResult> Refresh(RefreshDto refreshDto)
        {
            var priciple = GetClaimsPrincipalToken(refreshDto.AccessToken);
            if(priciple?.Identity?.Name is null)
            {
                _result.IsSuccess = false;
                _result.Message = "Missing access token to get new token";
                return _result;
            }
            var user = await _context.Users
                .Where(x => x.Username == priciple.Identity.Name)
                .FirstOrDefaultAsync();

            if(user?.RefreshToken != refreshDto.RefreshToken || user.ExpiredAt < DateTime.UtcNow)
            {
                _result.IsSuccess = false;
                _result.Message = "Refresh token expired";
                return _result;
            }
            var roles = await _context.UserRoles
                        .Where(x => x.UserId == user.Id)
                        .Select(x => x.Role.RoleName)
                        .ToListAsync();
            var AccessToken = _jwt.GenerateJWT(user, roles);
            _result.IsSuccess = true;
            _result.Data = AccessToken;
            _result.Message = "Create new token sucess";
            return _result;
        }

        public async Task<ModelResult> Register(RegisterDto registerDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (registerDto == null)
                    {
                        _result.IsSuccess = false;
                        _result.Message = "Missing parameter";
                        return _result;
                    }
                    var exists = await _context.Users
                        .AnyAsync(x => x.Email == registerDto.Email || x.Username == registerDto.Username);
                    if (exists)
                    {
                        _result.IsSuccess = false;
                        _result.Message = "The email or username has already been used by another user";
                        return _result;
                    }
                    var RoleEnum = (Data.Enums.Role)registerDto.RoleId;
                    var NewRoleName = RoleEnum.ToString();
                    var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == NewRoleName);
                    if (role == null)
                    {
                        role = new Data.Entities.Role { RoleName = NewRoleName };
                        await _context.Roles.AddAsync(role);
                        await _context.SaveChangesAsync();
                    }
                    var passwordHashed = Encrypt.EncryptMd5(registerDto.Password);
                    var newUser = _mapper.Map<User>(registerDto);
                    newUser.Password = passwordHashed;
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();
                    var newUserRole = new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = role.Id
                    };
                    await _context.UserRoles.AddAsync(newUserRole);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _result.IsSuccess = true;
                    _result.Message = "Create user success";
                    return _result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _result.IsSuccess = false;
                    _result.Message = $"An error occurred: {ex.Message}";
                    return _result;
                }
            }
        }
    }
}
