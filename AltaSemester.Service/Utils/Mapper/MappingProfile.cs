﻿using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.Manager;
using AltaSemester.Data.Dtos.Patient;
using AltaSemester.Data.Dtos.Service;
using AltaSemester.Data.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Utils.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Registration, User>()
                .ForMember(dest => dest.Username, otp => otp.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Password, otp => otp.Ignore());
            CreateMap<EditUserDto, User>()
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Username, otp => otp.MapFrom(src => src.Username))
                .ForMember(dest => dest.UserRole, otp => otp.MapFrom(src =>src.UserRole))
                .ForMember(dest => dest.FullName, otp => otp.MapFrom(src =>src.FullName))
                .ForMember(dest => dest.Password, otp => otp.Ignore());
            CreateMap<TicketDto, Assignment>()
                .ForMember(dest => dest.Code, opt => opt.Ignore());
            CreateMap<DeviceDto, Device>();
            CreateMap<ServiceDto, AltaSemester.Data.Entities.Service>();
        }
    }
}
