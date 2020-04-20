using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Occupancy
    {
        public int OccupancyID { get; set; }
        public OccupancyType OccupancyType { get; set; }
        public double? Value { get; set; }
        public UnitOfMeasure UoM { get; set; }
        public bool SeasonallyAdjusted { get; set; }

        [Required]
        public Time Time { get; set; }
    }

    public enum OccupancyType
    {
        Renter_Occupied,
        Owner_Occupied,
        Total_Occupied
    }
}
