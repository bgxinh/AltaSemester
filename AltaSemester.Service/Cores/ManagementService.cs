﻿using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AltaSemester.Service.Utils.Helper;
using AltaSemester.Service.Utils.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
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

        public async Task<ModelResult> EditUser(string token, string username, EditUserDto editUserDto)
        {
            ModelResult _result = new ModelResult();

            try
            {
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

    }
}
