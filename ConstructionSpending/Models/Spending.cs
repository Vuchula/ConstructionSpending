using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Spending
    {
        public int SpendingID { get; set; }
        public Time Time { get; set; } 
        public ConstructionType ConstructionType { get; set; } 
        public decimal? Value { get; set; }

    }

    public enum ConstructionType
    {
        Private,
        Public
    }
}
