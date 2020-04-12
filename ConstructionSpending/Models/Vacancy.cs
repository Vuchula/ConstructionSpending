using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Vacancy
    {
        public Time Time { get; set; } //Composite Key needed requires Fluent API
        public VacancyType VacancyType { get; set; } //Composite Key needed requires Fluent API
        public ICollection<Market> Markets { get; set; }
        public float? Value { get; set; }

    }

    public enum VacancyType
    {
        Year_Round,
        Seasonal
    }

    public class Market
    {
        public int MarketID { get; set; } 
        public MarketStatus MarketStatus { get; set; } 
        public MarketType? MarketType { get; set; }
        public bool? On_Contract { get; set; }
        public HeldOffType? HeldOffType { get; set; }

        public Vacancy Vacancy { get; set; }
    }

    public enum MarketStatus
    {
        Off_Market,
        On_Market,
    }

    public enum MarketType
    {
        Rent,
        Sale
    }

    public enum HeldOffType
    {
        Occasional_Use,
        Resides_Elsewhere,
        Other_Reasons
    }
}
