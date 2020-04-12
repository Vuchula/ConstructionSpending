using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Spending
    {
        public Time Time { get; set; } //Composite Key needed requires Fluent API
        public ConstructionType ConstructionType { get; set; } //Composite Key needed requires Fluent API
        public decimal? Value { get; set; }

    }

    public enum ConstructionType
    {
        Private,
        Public
    }
}
