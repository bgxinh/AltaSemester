using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Service.Cores.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class ManagementService : IManagementService
    {
        private readonly AltaSemesterContext _context;
        private ModelResult _result;
        public ManagementService(AltaSemesterContext context)
        {
            _context = context;
            _result = new ModelResult();
        }

        public Task<ModelResult> EditUser(string Username)
        {
            throw new NotImplementedException();
        }

        public Task<ModelResult> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}
