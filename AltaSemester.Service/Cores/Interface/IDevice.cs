﻿using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Patient;
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
        public Task<ModelResult> EditDevice(string deviceCode, DeviceDto deviceDto);
        public Task<ModelResult> DeleteDevice(string deviceCode);
        public Task<ModelResult> AddNewDevice(DeviceDto deviceDto);
        public Task<ModelResult> ImportDeviceFromExcel (FileImportRequest fileImportRequest);
        public Task<CountDto> CountDevices();
    }
}
