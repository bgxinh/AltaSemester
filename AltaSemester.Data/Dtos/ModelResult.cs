using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class ModelResult
    {
        public object? Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
