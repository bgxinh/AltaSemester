using AltaSemester.Data.DataAccess;
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
        public async Task<ModelResult> Registration(Registration registrationDto)
        {
            ModelResult _result = new ModelResult();
            using (var transaction = await _context.Database.BeginTransactionAsync()) 
            {
                try
                {
                    if (registrationDto == null) 
                    {
                        _result.Success = false;
                        _result.Message = "Missing parameter";
                        return _result;
                    }
                    var checkEmail = await _context.Users.AnyAsync(x => x.Email == registrationDto.Email);
                    var checkUser = await _context.Users.AnyAsync(x => x.Username == registrationDto.Username);
                    if (checkEmail || checkUser) 
                    {
                        _result.Success = false;
                        _result.Message = "The email or Username has already been used by another user";
                        return _result;
                    }
                    User user = _mapper.Map<User>(registrationDto);
                    user.Password = Encrypt.EncryptMd5(registrationDto.Password);
                    user.UserRole = null;
                    var result = await _context.Users.AddAsync(user);
                    if (result == null)
                    {
                        _result.Success = false;
                        _result.Message = "Create user failed";
                        return _result;
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _result.Success = true;
                    _result.Message = "Create user success";
                    return _result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _result.Success = false;
                    _result.Message = ex.Message;
                    return _result;
                }
            }
        }
        public async Task<ModelResult> Login (string username, string password)
        {
            ModelResult _result = new ModelResult();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    {
                        _result.Success = false;
                        _result.Message = "Missing username or password";
                        return _result;
                    }
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
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
                    if(user.RefreshToken == null || user.ExpiredAt > DateTime.UtcNow)
                    {
                        var refreshToken = RefreshToken.GenerateRefreshToken();
                        user.RefreshToken = refreshToken;
                        user.ExpiredAt = DateTime.UtcNow.AddDays(7);
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
                    };
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _result.Success = true;
                    _result.Data = _response;
                    _result.Message = "Login success";
                    return _result;
                }
                catch (Exception ex) 
                { 
                    transaction.Rollback();
                    _result.Success= false;
                    _result.Message = ex.Message;
                    return _result;
                }
            }
        }

        public async Task<ModelResult> Refresh(string accessToken, string refreshToken)
        {
            ModelResult _result = new ModelResult();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) {
                    _result.Message = "Missing access token or refresh token";
                    _result.Success = false;
                    return _result;
                }
                var priciple = RefreshToken.GetClaimsPrincipalToken(accessToken, _configuration);
                if (priciple?.Identity?.Name is null)
                {
                    _result.Message = "Access token is not valid";
                    _result.Success = false;
                    return _result;
                }
                var user = await _context.Users.Where(x => x.Username == priciple.Identity.Name).FirstOrDefaultAsync();
                if (user == null || user?.RefreshToken != refreshToken || user.ExpiredAt > DateTime.UtcNow)
                {
                    _result.Message = "Refresh token is expired";
                    _result.Success = false;
                    return _result;
                }
                var token = GenerateAccessToken(user);
                _result.Data = user;
                _result.Success = true;
                _result.Message = "Create new token success";
                return _result;
            }
        }
    }
}
