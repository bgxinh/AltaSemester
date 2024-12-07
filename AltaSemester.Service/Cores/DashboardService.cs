using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos.Manager;
using AltaSemester.Service.Cores.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class DashboardService : IDashboard
    {
        private AltaSemesterContext _context;
        public DashboardService(AltaSemesterContext context)
        {
            _context = context;
        }
        public List<int> GetDaysInCurrentMonth(int month, int year)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            List<int> days = new List<int>();
            for (int day = 1; day <= daysInMonth; day++)
            {
                days.Add(day);
            }
            return days;
        }



        public List<StatisticmonthDto> GetStatisticByMonth(int month)
        {
            var assignments = _context.Assignments.Where(x => x.AssignmentDate.Month == month && x.AssignmentDate.Year == DateTime.UtcNow.Year).ToList();
            List<StatisticmonthDto> myList = new List<StatisticmonthDto>();
            foreach (var item in GetDaysInCurrentMonth(month, DateTime.UtcNow.Year))
            {

                myList.Add(new StatisticmonthDto()
                {
                    day = item.ToString(),
                    value = assignments.Where(x => x.AssignmentDate.Day == item).Count()
                });
            }
            return myList;
        }
    }
}
