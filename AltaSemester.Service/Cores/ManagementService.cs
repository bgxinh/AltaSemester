using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Service.Utils.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit.Encodings;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class ManagementService : IManagementService
    {
        private readonly AltaSemesterContext _context;
        private ModelResult _result;
        private IMapper _mapper;
        private readonly IConfiguration _config;
        public ManagementService(AltaSemesterContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _result = new ModelResult();
            _mapper = mapper;
            _config = config;
        }

        public async Task<ModelResult> AddNewUser(Registration registrationDto)
        {
            ModelResult _result = new ModelResult();

            try
            {
                if (registrationDto == null)
                {
                    _result.Success = false;
                    _result.Message = "Missing parameter";
                    return _result;
                }

                if (string.IsNullOrWhiteSpace(registrationDto.Email) ||
                    string.IsNullOrWhiteSpace(registrationDto.Username) ||
                    string.IsNullOrWhiteSpace(registrationDto.Password))
                {
                    _result.Success = false;
                    _result.Message = "Invalid input data";
                    return _result;
                }

                var userExists = await _context.Users
                    .AnyAsync(x => x.Email == registrationDto.Email || x.Username == registrationDto.Username);
                if (userExists)
                {
                    _result.Success = false;
                    _result.Message = "The email or username has already been used by another user";
                    return _result;
                }
                User user = _mapper.Map<User>(registrationDto);
                user.Password = Encrypt.EncryptMd5(registrationDto.Password);
                user.UserRole = registrationDto.Role;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _result.Success = true;
                _result.Message = "User created successfully";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = $"An error occurred: {ex.Message}";
            }

            return _result;
        }
        public async Task<ModelResult> EditUser(string token, string username, EditUserDto editUserDto)
        {
            ModelResult _result = new ModelResult();

            try
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
                var principal = RefreshToken.GetClaimsPrincipalToken(token, _config);
                if (principal == null)
                {
                    _result.Success = false;
                    _result.Message = "Token not valid";
                    return _result;
                }

                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var currentUsername = principal.Identity?.Name;

                if (role != "Admin" && currentUsername != username)
                {
                    _result.Success = false;
                    _result.Message = "You are not authorized to edit other users.";
                    return _result;
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                if (user == null)
                {
                    _result.Success = false;
                    _result.Message = "User not found.";
                    return _result;
                }

                _mapper.Map(editUserDto, user);

                if (!string.IsNullOrEmpty(editUserDto.Password))
                {
                    user.Password = Encrypt.EncryptMd5(editUserDto.Password);
                }

                await _context.SaveChangesAsync();

                _result.Success = true;
                _result.Message = "User information updated successfully.";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = ex.Message;
            }

            return _result;
        }
        
        //Lấy ra tất cả user và assignment không phân trang
        public async Task<ModelResult> GetAllUsers()
        {
            ModelResult _result = new ModelResult();
            try
            {
                List<UserDto> list = await _context.Users.Select(user => new UserDto
                {
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserRole = user.UserRole,
                    Note = user.Note,
                    IsActive = user.IsActive,
                }).ToListAsync();
                _result.Success = true;
                _result.Message = "Get all user success";
                _result.Data = list;
                return _result;
            }
            catch (Exception ex) 
            {
                _result.Message = ex.Message;
                _result.Success = false;
                return _result;
            }
        }
        public async Task<ModelResult> GetAssignment()
        {
            ModelResult _result = new ModelResult();
            try
            {
                var list = await _context.Assignments.ToListAsync();
                _result.Data = list;
                _result.Success = true;
                _result.Message = "Get all assignments successfully";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = ex.Message;
            }

            return _result;
        }
        public async Task<ModelResult> DoctorGetAssignment(string token)
        {
            try
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
                var principal = RefreshToken.GetClaimsPrincipalToken(token, _config);
                var doctor = await _context.Users.Where(x => x.Username == principal.Identity.Name).FirstOrDefaultAsync();
                List<Assignment> assignments = await _context.Assignments.Where(x => x.ServiceCode == doctor.Note && x.ExpiredDate >= DateTime.UtcNow).ToListAsync();
                _result.Data = assignments;
                _result.Success = true;
                _result.Message = "Get assignment success";
                return _result;

            }
            catch (Exception ex)
            {
                _result.Message = ex.Message;
                _result.Success = false;
                return _result;
            }
        }

        //Lấy ra assignment có phân trang, điều kiện lọc cho staff và admin, còn doctor chỉ lấy trong ngày

        public async Task<ModelResult> DoctorGetAssignmentPage(string token, int pageNumber, int pageSize)
        {
            try
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
                var principal = RefreshToken.GetClaimsPrincipalToken(token, _config);
                var doctor = await _context.Users.Where(x => x.Username == principal.Identity.Name).FirstOrDefaultAsync();
                DateTime startOfDay = DateTime.UtcNow.Date;
                DateTime endOfDay = startOfDay.AddDays(1);
                List<Assignment> assignments = await _context.Assignments
                    .Where(x => x.ServiceCode == doctor.Note
                                && x.Status == 1
                                && x.ExpiredDate >= startOfDay
                                && x.ExpiredDate < endOfDay)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _result.Data = assignments;
                _result.Success = true;
                _result.Message = "Get assignment success";
                return _result;

            }
            catch (Exception ex)
            {
                _result.Message = ex.Message;
                _result.Success = false;
                return _result;
            }
        }
        public async Task<ModelResult> GetAssignmentPage(int pageNumber, int pageSize, GetAssignmentDto assignmentDto)
        {
            ModelResult _result = new ModelResult();
            try
            {
                if (assignmentDto == null)
                {
                    _result.Message = "Assignment filter data is required.";
                    _result.Success = false;
                    return _result;
                }
                if (pageNumber < 1 || pageSize < 1)
                {
                    _result.Message = "Page number and page size must be greater than 0.";
                    _result.Success = false;
                    return _result;
                }

                IQueryable<Assignment> query = _context.Assignments.AsQueryable();
                if (!string.IsNullOrEmpty(assignmentDto.ServiceCode))
                {
                    query = query.Where(x => x.ServiceCode == assignmentDto.ServiceCode);
                }

                if (!string.IsNullOrEmpty(assignmentDto.DeviceCode))
                {
                    query = query.Where(x => x.DeviceCode == assignmentDto.DeviceCode);
                }

                if (assignmentDto.Status != null)
                {
                    query = query.Where(x => x.Status == assignmentDto.Status);
                }

                if (assignmentDto.From != null)
                {
                    query = query.Where(x => x.ExpiredDate > assignmentDto.From);
                }

                if (assignmentDto.To != null)
                {
                    query = query.Where(x => x.ExpiredDate < assignmentDto.To);
                }
                var assignments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _result.Data = assignments;
                _result.Success = true;
                _result.Message = "Assignments retrieved successfully.";
            }
            catch (Exception ex)
            {
                _result.Message = ex.Message;
                _result.Success = false;
            }

            return _result;
        }

        //Lấy ra user trong quản lý user, có phân trang
        public async Task<ModelResult> GetUserPage(int pageNumber, int pageSize, string? role)
        {
            var _result = new ModelResult();
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    _result.Success = false;
                    _result.Message = "Page number and page size must be greater than zero.";
                    return _result;
                }
                IQueryable<User> query = _context.Users.AsQueryable();
                if(!string.IsNullOrEmpty(role))
                {
                    query = query.Where(x => x.UserRole  == role);
                }
                var list = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                _result.Data = _mapper.Map<List<UserDto>>(list);
                _result.Message = "Get user success";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = ex.Message;
            }
            return _result;
        }

        public async Task<ModelResult> DeleteUser(string username)
        {
            var _result = new ModelResult();
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    _result.Success = false;
                    _result.Message = "Missing username";
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                if(user == null)
                {
                    _result.Success = false;
                    _result.Message = "User dost not exits";
                    return _result;
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _result.Success = true;
                _result.Message = "Delete user sucess";
            }
            catch (Exception ex) 
            {
                _result.Success = false;
                _result.Message = ex.Message;
            }
            return _result;
        }

        //Thêm user bằng file excel
        public async Task<ModelResult> AddUserFormExcel(FileImportRequest fileImportRequest)
        {
            var _result = new ModelResult();
            if (fileImportRequest == null)
            {
                _result.Success = false;
                _result.Message = "Missing file";
                return _result;
            }
            if (!Path.GetExtension(fileImportRequest.formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                _result.Success = false;
                _result.Message = "Not support this file";
                return _result;
            }
            using (var transaction = await _context.Database.BeginTransactionAsync()) 
            {
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        fileImportRequest.formFile.CopyTo(stream);
                        if (stream.Length == 0)
                        {
                            _result.Success = false;
                            _result.Message = "Empty file";
                            return _result;
                        }
                        using (var reader = new ExcelPackage(stream)) 
                        {
                            if (reader.Workbook.Worksheets.Count == 0)
                            {
                                _result.Success = false;
                                _result.Message = "No worksheets found in the Excel file";
                                return _result;
                            }
                            ExcelWorksheet worksheet = reader.Workbook.Worksheets[0];
                            var count = worksheet.Dimension.Rows;
                            var exisingUsers = await _context.Users.
                                Select(x => new { x.Username, x.Email }).
                                ToListAsync();
                            var processedEmail = new HashSet<string>();
                            for (int row = 2; row <= count; row++) 
                            {
                                var email = worksheet.Cells[row, 5]?.Value?.ToString().Trim();
                                var username = worksheet.Cells[row, 3]?.Value?.ToString().Trim();
                                if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) 
                                    || exisingUsers.Any(x=> x.Username == username || x.Email == email)
                                    || processedEmail.Contains(email)
                                    )
                                {
                                    continue;
                                }
                                processedEmail.Add(email);
                                var user = new User
                                {
                                    FullName = worksheet.Cells[row, 2]?.Value?.ToString().Trim(),
                                    Username = username,
                                    Password = Encrypt.EncryptMd5(worksheet.Cells[row, 4]?.Value?.ToString().Trim()),
                                    Email = email,
                                    UserRole = worksheet.Cells[row, 6]?.Value?.ToString().Trim(),
                                    PhoneNumber = worksheet.Cells[row, 7]?.Value?.ToString().Trim(),
                                    Note = worksheet.Cells[row, 8]?.Value?.ToString().Trim(),
                                    CreateAt = DateTime.UtcNow
                                };
                                await _context.AddAsync(user);
                            }
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            _result.Success = true;
                            _result.Message = "Import user success";
                        }
                    }
                    catch (Exception ex) 
                    {
                        transaction.Rollback();
                        _result.Success = false;
                        _result.Message = ex.Message;
                    }
                }

            }
            return _result;
        }
    }
}
