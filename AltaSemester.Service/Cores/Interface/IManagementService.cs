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
        public Task<ModelResult> GetAllUsers();
        public Task<ModelResult> EditUser(string Username);
    }
}
