using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Occupancy
    {
        public int OccupancyID { get; set; }
        public OccupancyType OccupancyType { get; set; }
        public decimal? Value { get; set; }

        public int TimeID { get; set; } //declares as not null
        public Time Time { get; set; }
    }

    public enum OccupancyType
    {
        Renter_Occupied,
        Owner_Occupied
    }
}
