using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Spending
    {
        public int SpendingID { get; set; }
        public ConstructionType ConstructionType { get; set; } 
        public decimal? Value { get; set; }
        public UnitOfMeasure UoM { get; set; }
        public bool SeasonallyAdjusted { get; set; }

        [Required]
        public Time Time { get; set; }


    }

    public enum ConstructionType
    {
        Private,
        Public
    }

    public enum UnitOfMeasure
    {
        in_Percentage,
        in_Thousand_Units,
        in_Million_Dollars
    }
}
