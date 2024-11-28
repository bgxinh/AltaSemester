using AltaSemester.Data.Dtos.AuthDtos;
using AltaSemester.Data.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Utils.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Username, otp => otp.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src=> src.Email))
                .ForMember(dest => dest.FullName, otp => otp.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom( src => src.PhoneNumber))
                .ForMember(dest => dest.Password, otp => otp.Ignore())
                ;
        }
    }
}
