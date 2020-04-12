using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Occupancy
    {
        public Time Time { get; set; } //Composite Key needed requires Fluent API
        public OccupancyType OccupancyType { get; set; } //Composite Key needed requires Fluent API
        public float? Value { get; set; }
    }

    public enum OccupancyType
    {
        Renter_Occupied,
        Owner_Occupied
    }
}
