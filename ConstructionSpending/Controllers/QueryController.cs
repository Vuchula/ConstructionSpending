using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionSpending.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionSpending.DataAccess;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
            int startYear = 2015;
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
        public IActionResult PercentageChange()
        {
            //Using 2002 Q1 as base number we want to calculate the change in
            //1.Occupied Houses
            //2.Vacant Houses
            //3.Total Houses over time

            //declaring the base time = 2002, Q1, month = 0
            var min = dbContext.Times
                .OrderBy(time => time.Year).ThenBy(time => time.Quarter).ThenBy(time => time.Month)
                .Where(time => time.Year == 2002)
                .FirstOrDefault();
            //Console.WriteLine("Time: {0}, {1}, {2}", min.Year, min.Quarter, min.Month);

            //Fetch base value for each Occ
            var baseOccupied = dbContext.Occupancies
                .Where(value => value.Time == min && value.OccupancyType == (OccupancyType)2)
                .FirstOrDefault();
            //Console.WriteLine("Occupied Units: {0} {1} {2} {3}", baseOccupied.Time.Year, baseOccupied.Time.Quarter, baseOccupied.Time.Month, baseOccupied.Value);

            //Fetch base value for Vac
            var baseVacant = dbContext.Vacancies
                .Where(value => value.Time == min && value.VacancyType == (VacancyType)2)
                .FirstOrDefault();
            //Console.WriteLine("Vacant Units: {0} {1} {2} {3}", baseVacant.Time.Year, baseVacant.Time.Quarter, baseVacant.Time.Month, baseVacant.Value);

            //Fetch base value for Spending
            var baseSpend = dbContext.Spendings
                .Where(value => value.Time == min && value.ConstructionType == (ConstructionType)2)
                .FirstOrDefault();
            //Console.WriteLine("Cons. Spending: {0} {1} {2} {3}", baseSpend.Time.Year, baseSpend.Time.Quarter, baseSpend.Time.Month, baseSpend.Value);

            //Need to make percentage calculations

            //Occupied in percentage since 2002
            var percentOccupy = dbContext.Occupancies
                .Where(value => value.OccupancyType == (OccupancyType)2 && value.Time.Year > 2001)
                .OrderBy(value => value.Time.Year).ThenBy(value => value.Time.Quarter)
                .Select(value => new
                {
                    Year = value.Time.Year,
                    Quarter = value.Time.Quarter,
                    Percentage = ((value.Value - baseOccupied.Value) / baseOccupied.Value) * 100
                })
                .ToList();

            //Vacant in percentage since 2002
            var percentVacant = dbContext.Vacancies
                .Where(value => value.VacancyType == (VacancyType)2 && value.Time.Year > 2001)
                .OrderBy(value => value.Time.Year).ThenBy(value => value.Time.Quarter)
                .Select(value => new
                {
                    Year = value.Time.Year,
                    Quarter = value.Time.Quarter,
                    Percentage = ((value.Value - baseVacant.Value) / baseVacant.Value) * 100
                })
                .ToList();

            //Spending in percentage since 2002
            var percentSpend = dbContext.Spendings
                .Where(value => value.ConstructionType == (ConstructionType)2 && value.Time.Year > 2001
                && value.Time.Month == 0)
                .OrderBy(value => value.Time.Year).ThenBy(value => value.Time.Quarter)
                .Select(value => new
                {
                    Year = value.Time.Year,
                    Quarter = value.Time.Quarter,
                    Percentage = ((value.Value - baseSpend.Value) / baseSpend.Value) * 100
                })
                .ToList();

            //Toggle of between query data
            //var data = percentOccupy;
            //var data = percentVacant;
            var data = percentSpend;

            
            //List for date, values
            List<string> date = new List<string>();
            List<double> rate = new List<double>();

            //Add values to lists
            foreach (var value in data)
            {
                date.Add(value.Year.ToString() + value.Quarter.ToString());
                rate.Add((double)value.Percentage);
            }
            //Console.WriteLine();

            return new EmptyResult();
        }

        public IActionResult RawResults()
        {
            //Query for Total Spending in Quarters
            var spendingQuery = dbContext.Spendings
                .Where(s => s.ConstructionType == (ConstructionType)2 && s.Time.Month == 0
                && s.UoM == (UnitOfMeasure)2 && s.SeasonallyAdjusted == false)
                .Include(s => s.Time)
                .OrderBy(s => s.Time.Year).ThenBy(s => s.Time.Quarter)
                .ToList();

            //Query for Occupied Values in Quarters
            var occupiedQuery = dbContext.Occupancies
                .Where(o => o.OccupancyType == (OccupancyType)2 && o.UoM == (UnitOfMeasure)1 && o.SeasonallyAdjusted == false)
                .Include(o => o.Time)
                .OrderBy(o => o.Time.Year).ThenBy(o => o.Time.Quarter)
                .ToList();

            //Query for Vacancy Values in Quarters
            var vacancyQuery = dbContext.Vacancies
                .Where(v => v.VacancyType == (VacancyType)2 && v.UoM == (UnitOfMeasure)1 && v.SeasonallyAdjusted == false)
                .Include(v => v.Time)
                .OrderBy(v => v.Time.Year).ThenBy(v => v.Time.Quarter)
                .ToList();

            //Query for total Units in Quarters
            var totalUnitsQuery = occupiedQuery
                .Join(vacancyQuery, occ => occ.Time, vac => vac.Time,
                (occ, vac) => new
                {
                    Year = occ.Time.Year,
                    Quarter = occ.Time.Quarter,
                    Value = occ.Value + vac.Value //Total Value
                });

            //Four sources to toggle between
            //var data = spendingQuery;
            //var data = occupiedQuery;
            //var data = vacancyQuery;
            var data = totalUnitsQuery;

            //query to select year
            var selectYear = data
                .Where(d => d.Year == 2002)
                .ToList();

            foreach (var qValue in selectYear)
            {
                Console.WriteLine("{0} {1} {2}", qValue.Year, qValue.Quarter, qValue.Value);
            }

            return new EmptyResult();
        }

    }
}
