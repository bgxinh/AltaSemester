using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos.Manager;
using AltaSemester.Service.Cores.Interface;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<int>> GetDaysInCurrentMonth(int month, int year)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            return await Task.FromResult(Enumerable.Range(1, daysInMonth).ToList());
        }

        public async Task<List<DateRangeDto>> GetWeeksInMonth(int month, int year)
        {

            var daysInMonth = await GetDaysInCurrentMonth(month, year);


            List<DateRangeDto> myList = new List<DateRangeDto>();


            for (int i = 0; i < daysInMonth.Count; i += 7)
            {

                myList.Add(new DateRangeDto()
                {
                    StartDay = daysInMonth[i],
                    EndDay = Math.Min(daysInMonth[i] + 6, daysInMonth.Last())
                });
            }

            return myList;
        }

        // thong ke theo ngay
        public async Task<List<StatisticmonthDto>> GetStatisticByMonth(int month)
        {
            var assignments = await _context.Assignments.Where(x => x.AssignmentDate.Month == month && x.AssignmentDate.Year == DateTime.UtcNow.Year).ToListAsync();
            List<StatisticmonthDto> myList = new List<StatisticmonthDto>();
            foreach (var item in await GetDaysInCurrentMonth(month, DateTime.UtcNow.Year))
            {

                myList.Add(new StatisticmonthDto()
                {
                    Name = item.ToString(),
                    Value = assignments.Where(x => x.AssignmentDate.Day == item).Count()
                });
            }
            return myList;
        }
        // thong ke theo tuan
        public async Task<List<StatisticmonthDto>> GetStatisticByWeek(int month)
        {

            var assignments = await _context.Assignments
                .Where(x => x.AssignmentDate.Month == month && x.AssignmentDate.Year == DateTime.UtcNow.Year)
                .ToListAsync();


            var weeksInMonth = await GetWeeksInMonth(month, DateTime.UtcNow.Year);


            List<StatisticmonthDto> weeks = new List<StatisticmonthDto>();


            foreach (var item in weeksInMonth)
            {


                weeks.Add(new StatisticmonthDto
                {
                    Name = $"{weeks.Count + 1} ",
                    Value = assignments
                    .Where(x => x.AssignmentDate.Day >= item.StartDay && x.AssignmentDate.Day <= item.EndDay)
                    .Count()
                });
            }
            return weeks;
        }
        //thong ke theo thang
        public async Task<List<StatisticmonthDto>> GetStatisticByYear()
        {

            var assignments = await _context.Assignments
                .Where(x => x.AssignmentDate.Year == DateTime.UtcNow.Year)
                .ToListAsync();


            List<StatisticmonthDto> myList = new List<StatisticmonthDto>();


            for (int month = 1; month <= 12; month++)
            {

                int value = assignments
                    .Where(x => x.AssignmentDate.Month == month)
                    .Count();


                myList.Add(new StatisticmonthDto
                {
                    Name = $"{month}",
                    Value = value
                });
            }
            return myList;

        }
    }
}

