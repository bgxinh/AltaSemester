﻿using AltaSemester.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IEmailService
    {
        Task<ModelResult> SendMailActiveAccount(string email, string hashEmail);
    }
}
