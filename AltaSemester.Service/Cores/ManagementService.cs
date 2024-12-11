using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Manager;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Service.Utils.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit.Encodings;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
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
        private IHostingEnvironment _hostingEnvironment;
        public ManagementService(AltaSemesterContext context, IMapper mapper, IConfiguration config, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _hostingEnvironment = hostingEnvironment;
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
                user.IsFirstLogin = true;

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
        public async Task<ModelResult> EditUser(string token, EditUserDto editUserDto)
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

                if (role != "Admin" && currentUsername != editUserDto.Username)
                {
                    _result.Success = false;
                    _result.Message = "You are not authorized to edit other users.";
                    return _result;
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == editUserDto.Username);
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
                List<Assignment> assignments = await _context.Assignments.Where(x => x.ServiceCode == doctor.Note && x.ExpiredDate >= DateTime.UtcNow.AddHours(7)).ToListAsync();
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
                DateTime startOfDay = DateTime.UtcNow.AddHours(7).Date;
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
        public async Task<ModelResult> GetAssignmentPage(string ServiceCode, string From, string To, string DeviceCode, string Status, int pageNumber, int pageSize)
        {
            ModelResult _result = new ModelResult();
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    _result.Message = "Page number and page size must be greater than 0.";
                    _result.Success = false;
                    return _result;
                }

                IQueryable<Assignment> query = _context.Assignments.AsQueryable();
                if (!string.IsNullOrEmpty(ServiceCode) && ServiceCode != "___")
                {
                    query = query.Where(x => x.ServiceCode == ServiceCode);
                }

                if (!string.IsNullOrEmpty(DeviceCode) && DeviceCode != "___")
                {
                    query = query.Where(x => x.DeviceCode == DeviceCode);
                }

                if (!string.IsNullOrEmpty(Status) && Status != "___" && byte.TryParse(Status, out byte StatusValue))
                {
                    query = query.Where(x => x.Status == StatusValue);
                }

                if (!string.IsNullOrEmpty(From) && From != "___" && DateTime.TryParse(From, out DateTime Start))
                {
                    query = query.Where(x => x.ExpiredDate > Start);
                }

                if (!string.IsNullOrEmpty(To) && To != "___" && DateTime.TryParse(To, out DateTime End))
                {
                    query = query.Where(x => x.ExpiredDate < End);
                }
                query = query.Where(x => x.Status == 1);
                var assignments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(x => x.AssignmentDate)
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
                if(!string.IsNullOrEmpty(role) && role != "___")
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
                            var processdUsername = new HashSet<string>();
                            for (int row = 2; row <= count; row++) 
                            {
                                var email = worksheet.Cells[row, 5]?.Value?.ToString().Trim();
                                var username = worksheet.Cells[row, 3]?.Value?.ToString().Trim();
                                if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) 
                                    || exisingUsers.Any(x=> x.Username == username || x.Email == email)
                                    || processedEmail.Contains(email)
                                    || processdUsername.Contains(username)
                                    )
                                {
                                    continue;
                                }
                                processedEmail.Add(email);
                                processdUsername.Add(username);
                                var user = new User
                                {
                                    FullName = worksheet.Cells[row, 2]?.Value?.ToString().Trim(),
                                    Username = username,
                                    Password = Encrypt.EncryptMd5(worksheet.Cells[row, 4]?.Value?.ToString().Trim()),
                                    Email = email,
                                    UserRole = worksheet.Cells[row, 6]?.Value?.ToString().Trim(),
                                    PhoneNumber = worksheet.Cells[row, 7]?.Value?.ToString().Trim(),
                                    Note = worksheet.Cells[row, 8]?.Value?.ToString().Trim(),
                                    CreateAt = DateTime.UtcNow.AddHours(7),
                                    IsFirstLogin = true
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
        public async Task<ModelResult> UpdateAvatar(FileImportRequest fileImportRequest, string token)
        {
            var _result = new ModelResult();

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
                    _result.Message = "Invalid token. Please provide a valid authorization token.";
                    return _result;
                }

                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var currentUsername = principal.Identity?.Name;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == currentUsername);
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "Avatar");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (user.Avatar != null)
                {
                    string avatarFileName = Path.GetFileName(new Uri(user.Avatar).AbsolutePath);
                    string avatarPath = Path.Combine(path, avatarFileName);
                    File.Delete(avatarPath);
                }

                var typePicture = fileImportRequest.formFile.FileName.Substring(fileImportRequest.formFile.FileName.LastIndexOf('.') + 1).ToLower();
                if (typePicture != "jpeg" && typePicture != "png" && typePicture != "jpg")
                {
                    _result.Message = "Invalid file type. Only JPG and PNG formats are supported.";
                    _result.Success = false;
                    return _result;
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileImportRequest.formFile.FileName);
                var filePath = Path.Combine(path, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileImportRequest.formFile.CopyToAsync(fileStream);
                }

                var avatarUrl = Path.Combine("http://localhost:5000", "Avatar", fileName).Replace("\\", "/");
                user.Avatar = avatarUrl;
                await _context.SaveChangesAsync();

                _result.Message = "Avatar uploaded successfully.";
                _result.Success = true;
                return _result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _result.Message = "A concurrency error occurred. The data may have been modified or deleted by another user.";
                _result.Success = false;
            }
            catch (DbUpdateException ex)
            {
                _result.Message = "A database error occurred while saving the changes. Please try again.";
                _result.Success = false;
            }
            catch (Exception ex)
            {
                _result.Message = $"An error occurred while uploading the avatar: {ex.Message}. Please check your input and try again.";
                _result.Success = false;
            }

            return _result;
        }

        public async Task<ModelResult> ChangStatusAssignment(string AssignmentCode)
        {
            var _result = new ModelResult();
            try
            {
                var assignments = await _context.Assignments
                    .Where(x => x.Status == 0)
                    .OrderBy(x => x.Code)
                    .ToListAsync();
                foreach (var assignment in assignments)
                {
                    if (assignment.Code == AssignmentCode)
                    {
                        assignment.Status = 2;
                        break;
                    }
                    assignment.Status = 1;
                }
                await _context.SaveChangesAsync();
                _result.Success = true;
                _result.Message = "Change status success";
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Message = ex.Message;
            }
            return _result;
        }
    }
}
