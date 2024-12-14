using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Entities;
using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Service;
using AltaSemester.Service.Cores.Interface;
using AutoMapper;
using AltaSemester.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;
using System.Net.Security;
using System.Text.RegularExpressions;
using AutoMapper.QueryableExtensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using AltaSemester.Data.Dtos.Patient;

namespace AltaSemester.Service.Cores
{
    public class ServiceService : IService
    {
        private readonly AltaSemesterContext _context;
        private ModelResult _result;
        private IMapper _mapper;
        public ServiceService(AltaSemesterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ModelResult> AddNewService(ServiceDto serviceDto)
        {
            var _result = new ModelResult();
            try
            {
                if (serviceDto == null) 
                {
                    _result.Message = "Missing information";
                    _result.Success = false;
                    return _result;
                }
                var missingInput = new List<string>();
                if (string.IsNullOrEmpty(serviceDto.ServiceName)) missingInput.Add(nameof(serviceDto.ServiceName));
                if (string.IsNullOrEmpty(serviceDto.ServiceCode)) missingInput.Add(nameof(serviceDto.ServiceCode));
                if (string.IsNullOrEmpty(serviceDto.ServiceDescription)) missingInput.Add(nameof(serviceDto.ServiceDescription));
                if (missingInput.Count > 0)
                {
                    _result.Message = $"Missing required fields: {string.Join(", ", missingInput)}.";
                    _result.Success = false;
                    return _result;
                }
                var service = _mapper.Map<AltaSemester.Data.Entities.Service>(serviceDto);
                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();
                _result.Success = true;
                _result.Message = "Added new service";
                return _result;
            }
            catch (Exception ex) 
            {
                _result.Message = ex.Message;
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> DeleteService(string serviceCode)
        {
            var _result = new ModelResult();
            try
            { 
                if (serviceCode == null) 
                {
                    _result.Message = "Missng service code";
                    _result.Success = false;
                    return _result;
                }
                var service = await _context.Services.Where(x => x.ServiceCode.Trim() == serviceCode.Trim()).FirstOrDefaultAsync();
                if (service == null)
                {
                    _result.Success = false;
                    _result.Message = "Service not found";
                    return _result;
                }
                 _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                _result.Success = true;
                _result.Message = "Deteleted service";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _result.Message = "Concurrency conflict detected. The device might have been modified or deleted by another process.";
                _result.Success = false;
            }
            catch (DbUpdateException ex)
            {
                _result.Message = "A database error occurred. Please check the data and try again.";
                _result.Success = false;
            }
            catch (Exception ex)
            {
                _result.Message = "An unexpected error occurred. Please try again later.";
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> EditService(ServiceDto serviceDto)
        {
            var _result = new ModelResult();
            try
            {
                if (serviceDto == null) 
                {
                    _result.Message = "Missng information";
                    _result.Success= false;
                    return _result;
                }
                var missingInput = new List<string>();
                if(string.IsNullOrEmpty(serviceDto.ServiceName))    missingInput.Add(nameof(serviceDto.ServiceName));
                if(string.IsNullOrEmpty(serviceDto.ServiceCode))    missingInput.Add(nameof(serviceDto.ServiceCode));
                if(string.IsNullOrEmpty(serviceDto.ServiceDescription)) missingInput.Add(nameof(serviceDto.ServiceDescription));
                if (missingInput.Count > 0) 
                {
                    _result.Message = $"Missing required fields: {string.Join(", ", missingInput)}.";
                    _result.Success = false;
                    return _result;
                }
                var device = await _context.Services.Where(x => x.ServiceCode == serviceDto.ServiceCode).FirstOrDefaultAsync();
                if (device == null) 
                {
                    _result.Success = false;
                    _result.Message = "Service not fount to edit";
                    return _result;
                }
                device.ServiceName = serviceDto.ServiceName;
                device.ServiceDescription = serviceDto.ServiceDescription;
                await _context.SaveChangesAsync();
                _result.Message = "Edit successfully";
                _result.Success = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _result.Message = "Concurrency conflict detected. The device might have been modified or deleted by another process.";
                _result.Success = false;
            }
            catch (DbUpdateException ex)
            {
                _result.Message = "A database error occurred. Please check the data and try again.";
                _result.Success = false;
            }
            catch (Exception ex)
            {
                _result.Message = "An unexpected error occurred. Please try again later.";
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> GetService(bool? Status)
        {
            var _result = new ModelResult();
            try
            {
                IQueryable<AltaSemester.Data.Entities.Service> services = _context.Services;
                if (!await services.AnyAsync()) 
                {
                    _result.Message = "Service is empty";
                    _result.Success = true;
                    return _result;
                }
                if(Status != null)
                {
                    services = services.Where(x => x.Status == Status);
                }
                var list = await services.ToListAsync();
                _result.Message = "Get service success";
                _result.Success = true;
                _result.Data = list;
            }   
            catch (Exception ex) 
            {
                _result.Message = ex.Message;
                _result.Success = false;
            }
            return _result;
        }

        public Task<ModelResult> ImportServiceFromExcel(FileImportRequest fileImportRequest)
        {
            throw new NotImplementedException();
        }
        public async Task<CountDto> CountService()
        {
            CountDto _count = new CountDto
            {
                Total = await _context.Services.CountAsync(),
                Active = await _context.Services.Where(d => d.Status == true).CountAsync()
            };
            return _count;

        }
    }
}
