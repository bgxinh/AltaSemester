using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.AuthDtos
{
    public class RefreshDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken {  get; set; }
    }
}
