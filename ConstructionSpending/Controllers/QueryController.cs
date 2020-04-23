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
            //IEnumerable<Time> times = dbContext.Times.ToList();
            //IEnumerable<Spending> spending = dbContext.Spendings.ToList(); //remember in monthly, this one needs conversion
            //IEnumerable<Vacancy> vacancies = dbContext.Vacancies.ToList();
            //IEnumerable<Occupancy> occupancies = dbContext.Occupancies.ToList();

            //simple date range
            var simpleRange = dbContext.Times
                .Where(time => time.Year >= startYear && time.Year <= endYear)
                .OrderBy(time => time.Year).ThenBy(time => time.Quarter).ThenBy(time => time.Month)
                .ToList();

            //query of time range with Occup & Vacancy
            var timeRange = dbContext.Times
                .Where(time => time.Year >= startYear && time.Year <= endYear)
                .OrderBy(time => time.Year).ThenBy(time => time.Quarter)
                .Include(occup => occup.Occupancies)
                .Select(time => new { time.Year, time.Quarter, time.Occupancies, time.Vacancies })
                .ToList();

            foreach (var quarter in simpleRange)
            {
                Console.WriteLine("{0}, {1}, {2}", quarter.Year, quarter.Quarter, quarter.Month);
                //Console.WriteLine("Year: {0} Quarter: {1}, Occu: {2}, Vac: {3}", quarter.Year, quarter.Quarter
                    //, quarter.Occupancies, quarter.Vacancies);
            }

            return new EmptyResult();
        }
        public IActionResult SpendToQuarter()
        {
            for (int year = 2002; year < 2020; year++)
            {
                Spending quartSpen = new Spending();
            }

            var yearlySpending = dbContext.Spendings
                .Where(expense => expense.UoM == (UnitOfMeasure)2 && expense.SeasonallyAdjusted == false && expense.ConstructionType == (ConstructionType)2)
                .Where(expense => expense.Time.Month != 0)
                .GroupBy(expense => expense.Time.Quarter)
                .Select(x => new { Quarter = x.Key, Sum = x.Sum(expense => expense.Value)})
                .ToList();

            foreach (var quarter in yearlySpending)
            {
                Console.WriteLine("Quarter: {0}, Sum: {1}", quarter.Quarter, quarter.Sum);
            }
            return new EmptyResult();
        }

        public IActionResult PercentageChange()
        {
            //Using 2000 Q1 as base number we want to calculate the change in
            //1.Occupied Houses
            //2.Vacant Houses
            //3.Total Houses over time

            //declaring the base time = 2000, Q1, month = 0
            var min = dbContext.Times
                .OrderBy(time => time.Year).ThenBy(time => time.Quarter).ThenBy(time => time.Month)
                .Where(time=> time.Year == 2002)
                .FirstOrDefault();
            //Console.WriteLine("{0}, {1}, {2}", min.Year, min.Quarter, min.Month);

            //Fetch base value for each Occ, Vac, Spending
            var baseOccupied = dbContext.Occupancies
                .Where(value => value.Time == min && value.OccupancyType == (OccupancyType)2)
                .FirstOrDefault();
            //Console.WriteLine("{0} {1} {2} {3}", baseOccupied.Time.Year, baseOccupied.Time.Quarter, baseOccupied.Time.Month, baseOccupied.Value);

            var baseVacant = dbContext.Vacancies
                .Where(value => value.Time == min && value.VacancyType == (VacancyType)2)
                .FirstOrDefault();
            //Console.WriteLine("{0} {1} {2} {3}", baseVacant.Time.Year, baseVacant.Time.Quarter, baseVacant.Time.Month, baseVacant.Value);

            //Pull from CreateQuartSpend
            
            //Do base value for spending

            //Need to make percentage calculations

            //Repeat the above for each table

            return new EmptyResult();
        }
    }
}
