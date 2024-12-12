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
        public Task<List<int>> GetDaysInCurrentMonth(int month, int year);
        public Task<List<DateRangeDto>> GetWeeksInMonth(int month, int year);
        public Task<List<StatisticmonthDto>> GetStatisticByMonth(int month);
        public Task<List<StatisticmonthDto>> GetStatisticByWeek(int month);
        public Task<List<StatisticmonthDto>> GetStatisticByYear();
    }
}
