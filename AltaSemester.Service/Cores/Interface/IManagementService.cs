using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IManagementService
    {
        public Task<ModelResult> AddNewUser(Registration registration);
        public Task<ModelResult> GetAllUsers();
        public Task<ModelResult> EditUser(string token, string username, EditUserDto editUserDto);
        public Task<ModelResult> GetAssignment();
        public Task<ModelResult> DoctorGetAssignment(string token);
        public Task<ModelResult> DoctorGetAssignmentPage(string token, int pageNumber, int pageSize);
        public Task<ModelResult> GetAssignmentPage(int pageNumber, int pageSize, GetAssignmentDto assignmentDto);
        public Task<ModelResult> GetUserPage(int pageNumber, int pageSize, string role);
        public Task<ModelResult> DeleteUser(string username);
    }
}
