using AltaSemester.Data.Dtos;
using AltaSemester.Service.Cores.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class DeviceService : IDevice
    {
        public Task<ModelResult> AddNewDevice(DeviceDto deviceDto)
        {
            throw new NotImplementedException();
        }

        public Task<ModelResult> DeleteDevice(string deviceCode)
        {
            throw new NotImplementedException();
        }

        public Task<ModelResult> EditDevice(DeviceDto deviceDto)
        {
            throw new NotImplementedException();
        }

        public Task<ModelResult> GetAllDevice()
        {
            throw new NotImplementedException();
        }

        public Task<ModelResult> GetDevicePage(int pageNumber, int pageSize, bool? Status, bool? StatusConnect)
        {
            throw new NotImplementedException();
        }
    }
}
