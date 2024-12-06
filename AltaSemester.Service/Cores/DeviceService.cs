using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AltaSemester.Service.Utils.Helper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class DeviceService : IDevice
    {
        private IMapper _mapper;
        private AltaSemesterContext _context;
        private ModelResult _result;
        public DeviceService(IMapper mapper, AltaSemesterContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ModelResult> AddNewDevice(DeviceDto deviceDto)
        {
            var _result = new ModelResult();
            try
            {
                var missingFields = new List<string>();
                if (string.IsNullOrEmpty(deviceDto?.DeviceCode)) missingFields.Add(nameof(deviceDto.DeviceCode));
                if (string.IsNullOrEmpty(deviceDto?.DeviceIP)) missingFields.Add(nameof(deviceDto.DeviceIP));
                if (string.IsNullOrEmpty(deviceDto?.DeviceUsername)) missingFields.Add(nameof(deviceDto.DeviceUsername));
                if (string.IsNullOrEmpty(deviceDto?.DevicePassword)) missingFields.Add(nameof(deviceDto.DevicePassword));
                if (string.IsNullOrEmpty(deviceDto?.DeviceType)) missingFields.Add(nameof(deviceDto.DeviceType));

                if (missingFields.Any())
                {
                    _result.Message = $"Missing fields: {string.Join(", ", missingFields)}";
                    _result.Success = false;
                    return _result;
                }

                if (await _context.Devices.AnyAsync(d => d.DeviceCode == deviceDto.DeviceCode))
                {
                    _result.Message = "Device code already exists";
                    _result.Success = false;
                    return _result;
                }
                var device = _mapper.Map<Device>(deviceDto);
                await _context.Devices.AddAsync(device);
                await _context.SaveChangesAsync();

                _result.Message = "Add device success";
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Message = "An unexpected error occurred. Please try again.";
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> DeleteDevice(string deviceCode)
        {
            var _result = new ModelResult();
            try
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    _result.Message = "Device code is required to delete the device.";
                    _result.Success = false;
                    return _result;
                }

                var device = await _context.Devices.FirstOrDefaultAsync(d => d.DeviceCode == deviceCode);
                if (device == null)
                {
                    _result.Message = "Device not found with the given code.";
                    _result.Success = false;
                    return _result;
                }

                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
                _result.Message = "Device successfully deleted.";
                _result.Success = true;
            }
            catch (DbUpdateException dbEx)
            {
                _result.Message = "Unable to delete device due to database constraints.";
                _result.Success = false;
            }
            catch (Exception ex)
            {
                _result.Message = "An unexpected error occurred. Please try again later.";
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> EditDevice(string deviceCode, DeviceDto deviceDto)
        {
            var _result = new ModelResult();
            try
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    _result.Message = "Device code is required to update the device.";
                    _result.Success = false;
                    return _result;
                }
                if (deviceDto == null)
                {
                    _result.Message = "Device information is required to update the device.";
                    _result.Success = false;
                    return _result;
                }

                var missingFields = new List<string>();
                if (string.IsNullOrEmpty(deviceDto?.DeviceIP)) missingFields.Add(nameof(deviceDto.DeviceIP));
                if (string.IsNullOrEmpty(deviceDto?.DeviceUsername)) missingFields.Add(nameof(deviceDto.DeviceUsername));
                if (string.IsNullOrEmpty(deviceDto?.DevicePassword)) missingFields.Add(nameof(deviceDto.DevicePassword));
                if (string.IsNullOrEmpty(deviceDto?.DeviceType)) missingFields.Add(nameof(deviceDto.DeviceType));

                if (missingFields.Any())
                {
                    _result.Message = $"Missing required fields: {string.Join(", ", missingFields)}.";
                    _result.Success = false;
                    return _result;
                }
                var device = await _context.Devices.FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);
                if (device == null)
                {
                    _result.Message = "Device does not exist.";
                    _result.Success = false;
                    return _result;
                }

                if (device.DeviceCode != deviceDto.DeviceCode &&
                    await _context.Devices.AnyAsync(d => d.DeviceCode == deviceDto.DeviceCode))
                {
                    _result.Message = "The new device code is already in use.";
                    _result.Success = false;
                    return _result;
                }

                device.DeviceCode = deviceDto.DeviceCode;
                device.DeviceIP = deviceDto.DeviceIP;
                device.DeviceUsername = deviceDto.DeviceUsername;
                device.DevicePassword = deviceDto.DevicePassword;
                device.DeviceType = deviceDto.DeviceType;
                device.DeviceName = deviceDto.DeviceName;

                await _context.SaveChangesAsync();
                _result.Message = "Device updated successfully.";
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


        public async Task<ModelResult> GetAllDevice()
        {
            var _result = new ModelResult();
            try
            {
                List<Device> devices = await _context.Devices.ToListAsync();
                _result.Data = devices;
                _result.Success = true;
                _result.Message = "Get device success";
            }
            catch (Exception ex) 
            {
                _result.Message = ex.Message;
                _result.Success = false;
            }
            return _result;
        }

        public async Task<ModelResult> GetDevicePage(int pageNumber, int pageSize, bool? status, bool? statusConnect)
        {
            var result = new ModelResult();
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    result.Success = false;
                    result.Message = "Page number and page size must be greater than zero.";
                    return result;
                }
                var query = _context.Devices.AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(device => device.Status == status.Value);
                }

                if (statusConnect.HasValue)
                {
                    query = query.Where(device => device.StatusConnect == statusConnect.Value);
                }
                query = query.OrderByDescending(device => device.Status);

                var paginatedList = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                result.Data = paginatedList;
                result.Success = true;
                result.Message = "Devices retrieved successfully.";
            }
            catch (Exception ex)
            {
                result.Message = $"Error occurred: {ex.Message}";
                result.Success = false;
            }

            return result;
        }


        public async Task<ModelResult> ImportDeviceFromExcel(FileImportRequest fileImportRequest)
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
                            var exisingDevices = await _context.Devices.
                                Select(x => x.DeviceCode)
                                .ToListAsync();
                            var processedDevice = new HashSet<string>();
                            for (int row = 2; row <= count; row++)
                            {
                                var deviceCode = worksheet.Cells[row, 2]?.Value?.ToString().Trim();
                                if (string.IsNullOrEmpty(deviceCode)
                                    || processedDevice.Contains(deviceCode)
                                    || exisingDevices.Contains(deviceCode)
                                    )
                                {
                                    continue;
                                }
                                processedDevice.Add(deviceCode);
                                var device = new Device
                                {
                                    DeviceCode = worksheet.Cells[row, 2]?.Value?.ToString().Trim(),
                                    DeviceName = worksheet.Cells[row, 3]?.Value?.ToString().Trim(),
                                    DeviceType = worksheet.Cells[row, 4]?.Value?.ToString().Trim(),
                                    DeviceUsername = worksheet.Cells[row, 5]?.Value?.ToString().Trim(),
                                    DevicePassword = Encrypt.EncryptMd5(worksheet.Cells[row, 6]?.Value?.ToString().Trim()),
                                    DeviceIP = worksheet.Cells[row, 7]?.Value?.ToString().Trim(),
                                    CreateAt = DateTime.UtcNow
                                };
                                await _context.AddAsync(device);
                            }
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            _result.Success = true;
                            _result.Message = "Import device success";
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
    }
}
