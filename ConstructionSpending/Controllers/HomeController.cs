using ConstructionSpending.APIHandlerManager;
using ConstructionSpending.DataAccess;
using ConstructionSpending.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ConstructionSpending.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ApplicationDbContext dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            dbContext = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public Report getRawResults(int year)
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
            var spendingData = spendingQuery;
            var occupiedData = occupiedQuery;
            var vacantData = vacancyQuery;
            var totalUnitData = totalUnitsQuery;

            //query to select year
            var selectYearSpending = spendingData
                .Where(d => d.Time.Year == year)
                .ToList();
            var selectYearOccupied = occupiedData
                .Where(d => d.Time.Year == year)
                .ToList();
            var selectVaccant = vacantData
                .Where(d => d.Time.Year == year)
                .ToList();
            var selectTotalUnit = totalUnitData
                .Where(d => d.Year == year)
                .ToList();

            Report report = new Report();
            SpendingQ spendingQ = new SpendingQ();
            VaccancyQ vaccancyQ = new VaccancyQ();
            OccupancyQ occupancyQ = new OccupancyQ();
            TotalUnitsQ totalUnitsQ = new TotalUnitsQ();
            foreach (var qValue in selectYearSpending)
            {
                if (qValue.Time.Quarter == Quarter.Q1)
                    spendingQ.Q1 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q2)
                    spendingQ.Q2 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q3)
                    spendingQ.Q3 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q4)
                    spendingQ.Q4 = qValue.Value;
            }
            foreach (var qValue in selectYearOccupied)
            {
                if (qValue.Time.Quarter == Quarter.Q1)
                    occupancyQ.Q1 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q2)
                    occupancyQ.Q2 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q3)
                    occupancyQ.Q3 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q4)
                    occupancyQ.Q4 = qValue.Value;
            }
            foreach (var qValue in selectVaccant)
            {
                if (qValue.Time.Quarter == Quarter.Q1)
                    vaccancyQ.Q1 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q2)
                    vaccancyQ.Q2 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q3)
                    vaccancyQ.Q3 = qValue.Value;
                if (qValue.Time.Quarter == Quarter.Q4)
                    vaccancyQ.Q4 = qValue.Value;
            }
            foreach (var qValue in selectTotalUnit)
            {
                if (qValue.Quarter == Quarter.Q1)
                    totalUnitsQ.Q1 = qValue.Value;
                if (qValue.Quarter == Quarter.Q2)
                    totalUnitsQ.Q2 = qValue.Value;
                if (qValue.Quarter == Quarter.Q3)
                    totalUnitsQ.Q3 = qValue.Value;
                if (qValue.Quarter == Quarter.Q4)
                    totalUnitsQ.Q4 = qValue.Value;
            }
            report.spendingQ = spendingQ;
            report.occupancyQ = occupancyQ;
            report.vaccancyQ = vaccancyQ;
            report.totalUnitsQ = totalUnitsQ;
            report.year = year;
            return report;
        }

        public IActionResult Reports(String year)
        {
            dynamic mymodel = new ExpandoObject();
            Report report = new Report();
            if (year != null)
            {
                report = getRawResults(int.Parse(year));

            }
            mymodel.report = report;
            IList<int> time = new List<int>() { 2002,2003,2004,2005,2006,2007,2008,2009,
                2010,2011,2012,2013,2014,2015,2016,2017,2018,2019};
            mymodel.Year = time;
            //List<String> categories = new List<string>() { "Vaccancy", "Occupancy", "Spending", "Total Units" };
            //mymodel.Category = categories;
            return View(mymodel);
        }


        public IActionResult SaveThis(string year)
        {
            dynamic mymodel = new ExpandoObject();
            if (year != null)
            {
                SavedYears savedYear = new SavedYears();
                savedYear.savedYear = int.Parse(year);

                var Years = dbContext.Years.ToList();
                Boolean found = false;
                foreach (SavedYears savedyear in Years)
                {
                    if (savedyear.savedYear == int.Parse(year))
                    {
                        found = true;
                    }

                }
                if (found == false)
                {
                    dbContext.Years.Add(savedYear);
                    dbContext.SaveChanges();
                }
            }
            List<SavedYears> yearsToShow = dbContext.Years.ToList();
            List<Report> reports = new List<Report>();
            for (int i = 0; i < yearsToShow.Count(); i++)
            {
                reports.Add(getRawResults(yearsToShow[i].savedYear));
            }
            mymodel.reports = reports;
            return View(mymodel);

        }

        public IActionResult DeleteThis(string year)
        {
            dynamic mymodel = new ExpandoObject();
            if (year != null)
            {
                SavedYears years = dbContext.Years.Where(p => p.savedYear == int.Parse(year)).FirstOrDefault();
                dbContext.Years.Remove(years);
                dbContext.SaveChanges();
            }
            mymodel.year = year;
            return View(mymodel);
        }

        public IActionResult Graphs()
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

                if (value.Quarter.ToString().Equals("Q1"))
                {
                    date.Add(value.Year.ToString() + "-" + "01");
                }
                if (value.Quarter.ToString().Equals("Q2"))
                {
                    date.Add(value.Year.ToString() + "-" + "04");
                }
                if (value.Quarter.ToString().Equals("Q3"))
                {
                    date.Add(value.Year.ToString() + "-" + "07");
                }
                if (value.Quarter.ToString().Equals("Q4"))
                {
                    date.Add(value.Year.ToString() + "-" + "10");
                }

                //date.Add(value.Year.ToString() + value.Quarter.ToString());
                rate.Add((double)value.Percentage);
                Console.WriteLine("Year and Quarter:" + date);
                Console.WriteLine("Percentage:" + rate);
            }
            List<string> occupydate = new List<string>();
            List<double> occupyrate = new List<double>();
            var peroccupydata = percentOccupy;
            foreach (var value in peroccupydata)
            {

                if (value.Quarter.ToString().Equals("Q1"))
                {
                    occupydate.Add(value.Year.ToString() + "-" + "01");
                }
                if (value.Quarter.ToString().Equals("Q2"))
                {
                    occupydate.Add(value.Year.ToString() + "-" + "04");
                }
                if (value.Quarter.ToString().Equals("Q3"))
                {
                    occupydate.Add(value.Year.ToString() + "-" + "07");
                }
                if (value.Quarter.ToString().Equals("Q4"))
                {
                    occupydate.Add(value.Year.ToString() + "-" + "10");
                }

                //date.Add(value.Year.ToString() + value.Quarter.ToString());
                occupyrate.Add((double)value.Percentage);
            }
            List<string> vacantdate = new List<string>();
            List<double> vacantrate = new List<double>();
            var pervacantdata = percentVacant;
            foreach (var value in percentVacant)
            {

                if (value.Quarter.ToString().Equals("Q1"))
                {
                    vacantdate.Add(value.Year.ToString() + "-" + "01");
                }
                if (value.Quarter.ToString().Equals("Q2"))
                {
                    vacantdate.Add(value.Year.ToString() + "-" + "04");
                }
                if (value.Quarter.ToString().Equals("Q3"))
                {
                    vacantdate.Add(value.Year.ToString() + "-" + "07");
                }
                if (value.Quarter.ToString().Equals("Q4"))
                {
                    vacantdate.Add(value.Year.ToString() + "-" + "10");
                }

                //date.Add(value.Year.ToString() + value.Quarter.ToString());
                vacantrate.Add((double)value.Percentage);
            }
            dynamic model = new ExpandoObject();
            Console.WriteLine("Serialised json:" + JsonConvert.SerializeObject(date));
            model.date = JsonConvert.SerializeObject(date);
            model.rate = JsonConvert.SerializeObject(rate);
            model.vacantRate = JsonConvert.SerializeObject(vacantrate);
            model.occupantRate = JsonConvert.SerializeObject(occupyrate);
            return View(model);

        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult ThankYou()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
