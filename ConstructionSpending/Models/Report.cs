using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Report
    {
        public SpendingQ spendingQ { get; set; }
        public VaccancyQ vaccancyQ { get; set; }
        public OccupancyQ occupancyQ { get; set; }
        public TotalUnitsQ totalUnitsQ { get; set; }
        public int year { get; set; }
    }

    public class SpendingQ
    {
        public double? Q1 { get; set; }
        public double? Q2 { get; set; }
        public double? Q3 { get; set; }
        public double? Q4 { get; set; }
    }

    public class VaccancyQ
    {
        public double? Q1 { get; set; }
        public double? Q2 { get; set; }
        public double? Q3 { get; set; }
        public double? Q4 { get; set; }
    }
    public class OccupancyQ
    {
        public double? Q1 { get; set; }
        public double? Q2 { get; set; }
        public double? Q3 { get; set; }
        public double? Q4 { get; set; }
    }
    public class TotalUnitsQ
    {
        public double? Q1 { get; set; }
        public double? Q2 { get; set; }
        public double? Q3 { get; set; }
        public double? Q4 { get; set; }
    }

}
