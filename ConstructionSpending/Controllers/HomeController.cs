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

        public IActionResult CleanVIP()
        {
            /*//Parameters
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

            foreach (ResponseVip expense in spendingQuery)
            {

                if ((expense.category_code == residential || expense.category_code == resAdjusted) &&
                    (expense.data_type_code == publicCons || expense.data_type_code == privateCons || expense.data_type_code == totalCons ||
                     expense.data_type_code == monPercPublic || expense.data_type_code == monPercPrivate || expense.data_type_code == monPercTotal))
                {
                    dbContext.Spendings.Add(filter(expense));
                }
            }
            dbContext.SaveChanges();*/

            IOrderedEnumerable<Spending> expenses = dbContext.Spendings
                .Include(t => t.Time)
                .ToList()
                .OrderBy(e => e.Time.Year)
                .ThenBy(e => e.Time.Month)
                .ThenBy(e => e.ConstructionType);
            return View(expenses);
        }

        Time datetable(DateTime date)
        {
            Time timeQuery = dbContext.Times
                .Where(t => (t.Year == date.Year) && (t.Month == (Month)date.Month))
                .Where(t => t.Quarter == (Quarter)GetQuarter(date)).FirstOrDefault();
            return timeQuery;

        }

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
