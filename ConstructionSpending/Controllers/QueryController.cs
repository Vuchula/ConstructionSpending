using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionSpending.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionSpending.DataAccess;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ConstructionSpending.Controllers
{
    public class QueryController : Controller
    {
        public ApplicationDbContext dbContext;
        private ILogger<QueryController> _logger;

        public QueryController(ILogger<QueryController> logger, ApplicationDbContext context)
        {
            dbContext = context;
            _logger = logger;
        }

        public IActionResult DateRange()
        {
            //create func/query to get range of selected years
            
            //date range, will serve as input
            int startYear =  2015;
            int endYear = 2019;
            
            //tables used for querying
            IEnumerable<Time> times = dbContext.Times.ToList();
            IEnumerable<Spending> spending = dbContext.Spendings.ToList(); //remember in monthly, this one needs conversion
            IEnumerable<Vacancy> vacancies = dbContext.Vacancies.ToList();
            IEnumerable<Occupancy> occupancies = dbContext.Occupancies.ToList();

            //query of time range
            var timeRange = dbContext.Times
                .Where(time => time.Year >= startYear && time.Year <= endYear)
                .Include(occup => occup.Occupancies)
                .Select(time => new { time.Year, time.Quarter, time.Occupancies, time.Vacancies })
                .ToList();

            foreach (var quarter in timeRange)
            {
                Console.WriteLine("Year: {0} Quarter: {1}, Occu: {2}, Vac: {3}", quarter.Year, quarter.Quarter
                    , quarter.Occupancies, quarter.Vacancies);
            }


            return new EmptyResult();
        }
        public IActionResult Quarter()
        {
            //query to group values by quarter
            var yearlySpending = dbContext.Spendings
                .Where(expense => expense.Time.Year == 2019)
                .GroupBy(expense => ((int)expense.Time.Month - 1) / 3)
                .Select(x => new { Quarter = x.Key, Sum = x.Sum(expense => expense.Value) })
                .ToList();

            foreach (var quarter in yearlySpending)
            {
                Console.WriteLine("Quarter: {0}, Sum: {1:c}", quarter.Quarter + 1, quarter.Sum);
            }

            return new EmptyResult();
        }
    }
}
