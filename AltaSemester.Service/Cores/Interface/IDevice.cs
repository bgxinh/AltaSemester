using AltaSemester.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IDevice
    {
        public Task<ModelResult> GetAllDevice();
        public Task<ModelResult> GetDevicePage(int pageNumber, int pageSize, bool? Status, bool? StatusConnect);
        public Task<ModelResult> EditDevice(DeviceDto deviceDto);
        public Task<ModelResult> DeleteDevice(string deviceCode);
        public Task<ModelResult> AddNewDevice(DeviceDto deviceDto);
    }
}
