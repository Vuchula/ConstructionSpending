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
                Console.WriteLine("Quarter: {0}, Sum: {1}", quarter.Quarter + 1, quarter.Sum);
            }

            return new EmptyResult();
        }
    }
}
