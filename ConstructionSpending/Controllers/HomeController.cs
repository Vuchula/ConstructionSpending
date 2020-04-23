using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConstructionSpending.Models;
using ConstructionSpending.APIHandlerManager;
using ConstructionSpending.DataAccess;
using System.Net.Http;
using System.Data.Entity.Core.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.Tracing;

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

        //Save data for HV 
        public IActionResult SaveData()
        {
            //After running for the first time comment this part
            //Start comment here
            HttpClient httpClient;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            APIHandler api = new APIHandler();
            for (int i = 2000; i < 2020; i++)
            {
                //Gets the API data year wise starting from 2000
                List<Response> responses = api.GetData<List<Response>>("hv", i.ToString(), httpClient);
                //Iterate through each year's response and insert it into the table.
                foreach (Response response in responses)
                {
                    dbContext.Responses.Add(response);
                }
            }
            dbContext.SaveChanges();
            //End comment here
            IOrderedEnumerable<Response> rows = dbContext.Responses.ToList().OrderBy(r => r.time_slot_date);
            return View(rows);
        }

        public IActionResult SaveVIPData()
        {
            //After running for the first time comment this part
            //Start comment here
            HttpClient httpClient;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            APIHandler api = new APIHandler();
            for (int i = 2000; i < 2020; i++)
            {
                List<ResponseVip> responses = api.GetData<List<ResponseVip>>("vip", i.ToString(), httpClient);
                foreach (ResponseVip response in responses)
                {
                    dbContext.ResponseVips.Add(response);
                }
            }
            dbContext.SaveChanges();
            //End comment here
            IOrderedEnumerable<ResponseVip> rows = dbContext.ResponseVips.ToList().OrderBy(r => r.time_slot_date);
            return View(rows);
        }

        public IActionResult CreateTimeTable()
        {
            //Start Comment here
            for (int y = 2000; y < 2020; y++)
            {
                int counter = 0;
                for (int i = 0; i < 16; i++)
                {
                    int remainder = i % 4;
                    int m;
                    if (remainder == 0)
                    {
                        counter++;
                        m = 0;
                    }
                    else
                    {
                        m = i - (counter - 1);
                    };
                    var time = new Time()
                    {
                        Year = y,
                        Month = (Month)m,
                        Quarter = (Quarter)counter,
                    };
                    dbContext.Add(time);
                }
            }
            dbContext.SaveChanges();
            //End Comment
            IOrderedEnumerable<Time> times = dbContext.Times.ToList()
                .OrderBy(time => time.Year)
                .ThenBy(time => time.Month);
            return View(times);
        }

        //function to get monthly time(Spending VIP)
        Time datetable(DateTime date)
        {
            Time timeQuery = dbContext.Times
                .Where(t => (t.Year == date.Year) && (t.Month == (Month)date.Month))
                .Where(t => t.Quarter == (Quarter)GetQuarter(date)).FirstOrDefault();
            return timeQuery;

        }

        //function to get quarters (Spending VIP)
        int GetQuarter(DateTime date)
        {
            if (date.Month >= 1 && date.Month <= 3)
                return 1;
            else if (date.Month >= 4 && date.Month <= 6)
                return 2;
            else if (date.Month >= 7 && date.Month <= 9)
                return 3;
            else
                return 4;
        }
        //function to get quarterly time (HV Occup & Vac)
        Time hvDatetable(DateTime date, string quarter)
        {
            Time quarterQuery = dbContext.Times
                .Where(t => (t.Year == date.Year) && t.Quarter == (Quarter)hvGetQuarter(quarter))
                .Where(t => t.Month == 0)
                .FirstOrDefault();
            return quarterQuery;
        }

        //function to identify quarter time (HV Occup & Vac)
        int hvGetQuarter(string quarter)
        {
            if (quarter.ToLower().Contains("q1"))
            {
                return 1;
            }
            else if (quarter.ToLower().Contains("q2"))
            {
                return 2;
            }
            else if (quarter.ToLower().Contains("q3"))
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public IActionResult CleanVIP()
        {
            //Parameters
            string residential = "00XX";
            string resAdjusted = "A00XX"; // if this is present seasonally_adj should be "yes"
            string privateCons = "V";
            string publicCons = "P";
            string totalCons = "T";
            string monPercPublic = "MPCP";
            string monPercPrivate = "MPCV"; //not adding "E_" variability of data
            string monPercTotal = "MPCT";

            //Query
            IList<ResponseVip> spendingQuery = dbContext.ResponseVips.ToList();

            //filter function selects every record and interprets results
            Spending filter(ResponseVip expense)
            {
                Spending result = new Spending();
                //check ConstructionType & UoM
                if (expense.data_type_code == privateCons)
                {
                    result.ConstructionType = (ConstructionType)0;
                    result.UoM = (UnitOfMeasure)2;
                }
                else if (expense.data_type_code == publicCons)
                {
                    result.ConstructionType = (ConstructionType)1;
                    result.UoM = (UnitOfMeasure)2;
                }
                else if (expense.data_type_code == totalCons)
                {
                    result.ConstructionType = (ConstructionType)2;
                    result.UoM = (UnitOfMeasure)2;
                }
                else if (expense.data_type_code == monPercPrivate)
                {
                    result.ConstructionType = (ConstructionType)0;
                    result.UoM = (UnitOfMeasure)0;
                }
                else if (expense.data_type_code == monPercPublic)
                {
                    result.ConstructionType = (ConstructionType)1;
                    result.UoM = (UnitOfMeasure)0;
                }
                else if (expense.data_type_code == monPercTotal)
                {
                    result.ConstructionType = (ConstructionType)2;
                    result.UoM = (UnitOfMeasure)0;
                }
                //Value
                result.Value = expense.cell_value;
                //check season adj
                result.SeasonallyAdjusted = (expense.seasonally_adj == "yes") ? true : false;
                //Time
                result.Time = datetable(expense.time_slot_date);
                return result;
            };

            //filter & add to database
            foreach (ResponseVip expense in spendingQuery)
            {

                if ((expense.category_code == residential || expense.category_code == resAdjusted) &&
                    (expense.data_type_code == publicCons || expense.data_type_code == privateCons || expense.data_type_code == totalCons ||
                     expense.data_type_code == monPercPublic || expense.data_type_code == monPercPrivate || expense.data_type_code == monPercTotal))
                {
                    dbContext.Spendings.Add(filter(expense));
                }
            }
            dbContext.SaveChanges();

            IOrderedEnumerable<Spending> expenses = dbContext.Spendings
                .Include(t => t.Time)
                .ToList()
                .OrderBy(e => e.Time.Year)
                .ThenBy(e => e.Time.Month)
                .ThenBy(e => e.ConstructionType);
            return View(expenses);
        }

        Time SearchQuartTime(int year, Quarter quarter)
        {
            Time matchQuarter = dbContext.Times
                .Where(time => time.Month == 0 && time.Year == year && time.Quarter == quarter)
                .FirstOrDefault();
            return matchQuarter;
        }
        public IActionResult Spending2Quarter()
        {
            for (int year = 2002; year < 2020; year++)
            {
                //query to transform monthly values to quarters
                var quarterlySpending = dbContext.Spendings
                    .Where(expense => expense.UoM == (UnitOfMeasure)2 && expense.SeasonallyAdjusted == false
                    && expense.ConstructionType == (ConstructionType)2)
                    .Where(expense => expense.Time.Year == year && expense.Time.Month != 0)
                    .GroupBy(expense => expense.Time.Quarter)
                    .Select(x => new { Quarter = x.Key, Sum = x.Sum(expense => expense.Value) })
                    .ToList();
                foreach (var quarter in quarterlySpending)
                {
                    //Creating new Spending for quarter
                    Spending quartSpent = new Spending()
                    {
                        //Assigning Total constType, millions UoM, not season adjusted
                        ConstructionType = (ConstructionType)2,
                        UoM = (UnitOfMeasure)2,
                        SeasonallyAdjusted = false,
                    };
                    //Assigning new quarterly value
                    quartSpent.Value = quarter.Sum;
                    //assigning correct timeID
                    quartSpent.Time = SearchQuartTime(year, quarter.Quarter);
                    //Save Values
                    dbContext.Add(quartSpent);
                    //Display Saved Values on Output
                    Console.WriteLine("Successfully added ID: {0} {1} {2} {3} {4} {5}", quartSpent.Time.TimeID, quartSpent.Time.Year,
                        quartSpent.Time.Quarter, quartSpent.Time.Month, quartSpent.Value, quartSpent.ConstructionType);
                }
            }
            dbContext.SaveChanges();
            Console.WriteLine("Spending Table Updated!");
            return new EmptyResult();
        }

        //parameters HV data in Response
        //string percentage = "RATE"; //ignoring "E_" Sampling Variability
        string unit_count = "ESTIMATE";
        //occupied
        string homeOwnRate = "HOR";
        string homeOwnRateAdj = "SAHOR";
        string ownOccHouses = "OWNOCC";
        string rentOccHouses = "RNTOCC";
        string totOccupied = "OCC";
        //vacant 
        string rentVacRate = "RVR";
        string ownVacRate = "HVR";
        string totVacant = "VACANT";
        //string yearlyVac = "YRVAC";
        string seasonVac = "SEASON";
        string rentalVac = "RENT";
        string saleVac = "SALE";
        string rentedsoldVac = "RNTSLD";
        //off-market
        //string totOffMarket = "OFFMAR"; // sum of occasUse, resElsewhere, otherReason
        string occasUse = "OCCUSE";
        string resElsewhere = "URE";
        string otherReason = "OTH";
        //total units
        //string total_units = "TOTAL"; //sum of totOccupied & totVacant

        /*public IActionResult CreateMarket()
        {
            Market occasionalUse = new Market()
            {
                MarketStatus = 0,
                HeldOffType = 0
            };
            Market resideElsewhere = new Market()
            {
                MarketStatus = 0,
                HeldOffType = (HeldOffType)1
            };
            Market otherReason = new Market()
            {
                MarketStatus = 0,
                HeldOffType = (HeldOffType)2
            };
            Market onContract = new Market()
            {
                MarketStatus = (MarketStatus)1,
                On_Contract = true
            };
            Market forRent = new Market()
            {
                MarketStatus = (MarketStatus)1,
                MarketType = 0
            };
            Market forSale = new Market()
            {
                MarketStatus = (MarketStatus)1,
                MarketType = (MarketType)1
            };
            //add Db
            dbContext.Add(occasionalUse);
            dbContext.Add(resideElsewhere);
            dbContext.Add(otherReason);
            dbContext.Add(onContract);
            dbContext.Add(forRent);
            dbContext.Add(forSale);
            dbContext.SaveChanges();

            return new EmptyResult();
        }*/
        public IActionResult CreateOccupy()
        {
            //query
            IList<Response> housesQuery = dbContext.Responses.ToList();

            //create classification function
            Occupancy occupiedFilter(Response value)
            {
                Occupancy result = new Occupancy();
                //checking season_adjusted
                result.SeasonallyAdjusted = value.seasonally_adj == "yes" ? true : false;
                //UoM for rates & house units
                result.UoM = (value.category_code == unit_count) ? (UnitOfMeasure)1 : 0;
                //check occupancy type 
                if (value.data_type_code == ownOccHouses || value.data_type_code == homeOwnRate || value.data_type_code == homeOwnRateAdj)
                {
                    result.OccupancyType = (OccupancyType)1;
                }
                else if (value.data_type_code == rentOccHouses)
                {
                    result.OccupancyType = 0;
                }
                else
                {
                    result.OccupancyType = (OccupancyType)2;
                }
                //assign value
                result.Value = value.cell_value;
                //assign correct time (quarterly nums only)
                result.Time = hvDatetable(value.time_slot_date, value.time);
                return result;
            }

            //add to database
            foreach (Response value in housesQuery)
            {
                //US-level Occupied add to db
                if ((value.geo_level_code == "US") && (value.data_type_code == homeOwnRate || value.data_type_code == homeOwnRateAdj ||
                    value.data_type_code == ownOccHouses || value.data_type_code == rentOccHouses ||
                    value.data_type_code == totOccupied))
                {
                    dbContext.Occupancies.Add(occupiedFilter(value));
                }
            }
            //Save results
            dbContext.SaveChanges();
            //Create list for db
            IOrderedEnumerable<Occupancy> occupancies = dbContext.Occupancies.Include(t => t.Time)
                .ToList()
                .OrderBy(o => o.Time.Year)
                .ThenBy(o => o.Time.Quarter);

            return View(occupancies);
        }
        public IActionResult CreateVacancy()
        {
            //query
            IList<Response> housesQuery = dbContext.Responses.ToList();

            //create classification function
            Vacancy vacancyFilter(Response value)
            {
                Vacancy result = new Vacancy();
                //check season_adj
                result.SeasonallyAdjusted = (value.seasonally_adj == "yes") ? true : false;
                //check UoM
                result.UoM = (value.category_code == unit_count) ? (UnitOfMeasure)1 : 0;
                //check vacancy type
                if (value.data_type_code == totVacant)
                {
                    result.VacancyType = (VacancyType)2;
                }
                else if (value.data_type_code == seasonVac)
                {
                    result.VacancyType = (VacancyType)1;
                }
                else
                {
                    result.VacancyType = 0;
                    // assign market categories
                    if (value.data_type_code == occasUse || value.data_type_code == resElsewhere || value.data_type_code == otherReason)
                    {
                        //create new corresponding Market
                        Market market = new Market();
                        //mark as off-market
                        market.MarketStatus = 0;
                        //mark type of held-off
                        if (value.data_type_code == occasUse)
                        {
                            market.HeldOffType = 0;
                        }
                        else if (value.data_type_code == resElsewhere)
                        {
                            market.HeldOffType = (HeldOffType)1;
                        }
                        else
                        {
                            market.HeldOffType = (HeldOffType)2;
                        }

                        dbContext.Markets.Add(market);
                        dbContext.SaveChanges();
                        result.Market = market;
                    }
                    else if (value.data_type_code == rentalVac || value.data_type_code == saleVac || value.data_type_code == rentedsoldVac
                          || value.data_type_code == rentVacRate || value.data_type_code == ownVacRate)
                    {
                        //create new market 
                        Market market = new Market();
                        //mark as on market
                        market.MarketStatus = (MarketStatus)1;
                        //mark market-type
                        if (value.data_type_code == rentedsoldVac)
                        {
                            //on contract
                            market.On_Contract = true;
                        }
                        else
                        {
                            //not on contract
                            market.On_Contract = false;
                            //sale or rent market-type
                            market.MarketType = (value.category_code == saleVac || value.category_code == ownVacRate) ? (MarketType)1 : 0;
                        }
                        dbContext.Markets.Add(market);
                        dbContext.SaveChanges();
                        result.Market = market;
                    }
                }
                
                //assign value
                result.Value = value.cell_value;
                //assign time
                result.Time = hvDatetable(value.time_slot_date, value.time);
                return result;
            }

            //add to database
            foreach (Response value in housesQuery)
            {
                //US-level Vacant add to db
                if ((value.geo_level_code == "US") && (value.data_type_code == rentVacRate || value.data_type_code == ownVacRate ||
                    value.data_type_code == totVacant || //value.data_type_code == yearlyVac ||
                    value.data_type_code == seasonVac || value.data_type_code == rentalVac ||
                    value.data_type_code == saleVac || value.data_type_code == rentedsoldVac ||
                    value.data_type_code == occasUse || value.data_type_code == resElsewhere ||
                    value.data_type_code == otherReason))
                {
                    dbContext.Vacancies.Add(vacancyFilter(value));
                }
            }
            //Save results
            dbContext.SaveChanges();
            //Create list for db
            IOrderedEnumerable<Vacancy> vacancies = dbContext.Vacancies.Include(t => t.Time)
                .ToList()
                .OrderBy(v => v.Time.Year)
                .ThenBy(v => v.Time.Quarter);

            return View(vacancies);
        }
        public IActionResult Index()
        {

            /*APIHandler api = new APIHandler();
            List<Response> responses = api.GetHVData();
            foreach (Response response in responses)
            {
                dbContext.Responses.Add(response);
            }
            dbContext.SaveChanges();*/
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult Graphs()
        {
            return View();
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
