using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Time
    {
        public int TimeID { get; set; }
        public int Year { get; set; }
        public Quarter Quarter { get; set; }
        public Month Month { get; set; }

        public ICollection<Spending> Spendings { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Occupancy> Occupancies { get; set; }
    }

    public enum Quarter
    {
        Q1 = 1,
        Q2 = 2,
        Q3 = 3,
        Q4 = 4
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3, 
        April = 4,
        May = 5, 
        June = 6,
        July = 7,
        August = 8, 
        September = 9,
        October = 10, 
        November = 11,
        December = 12
    }
}
