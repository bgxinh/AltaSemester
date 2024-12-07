using AltaSemester.Data.Dtos.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IDashboard
    {
        public List<int> GetDaysInCurrentMonth(int month, int year);
        public List<StatisticmonthDto> GetStatisticByMonth(int month);
    }
}
