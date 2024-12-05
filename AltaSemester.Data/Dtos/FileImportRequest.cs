using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class FileImportRequest
    {
        public IFormFile formFile {  get; set; }
    }
}
