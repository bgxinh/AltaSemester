using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaSemester.Data.Dtos.Service;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using AltaSemester.Data.Dtos.Patient;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IService
    {
        public Task<ModelResult> GetService(bool? Status);
        public Task<ModelResult> EditService(ServiceDto serviceDto);
        public Task<ModelResult> DeleteService(string serviceCode);
        public Task<ModelResult> AddNewService(ServiceDto serviceDto);
        public Task<ModelResult> ImportServiceFromExcel(FileImportRequest fileImportRequest);
        public Task<CountDto> CountService();
        public Task<ModelResult> ChangeStatusService(string serviceCode);
    }
}
