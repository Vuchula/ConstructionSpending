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
            /*//Start comment here
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
            //End comment here*/
            List<Response> rows = dbContext.Responses.ToList();
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
            List<ResponseVip> rows = dbContext.ResponseVips.ToList();
            return View(rows);
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
