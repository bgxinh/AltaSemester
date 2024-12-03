using AltaSemester.Data.DataAccess;
using AltaSemester.Service.Cores.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Data.Entities;
using AltaSemester.Data.Dtos;
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
                    var user = await _context.Users.FindAsync(username);
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
                    LoginResponse _response = new LoginResponse
                    {
                        Username = user.Username,
                        Fullname = user.FullName,
                        Email = user.Email,
                        Note = user.Note,
                        Token = "token",
                        RefreshToken = "refresh token",
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
    }
}
