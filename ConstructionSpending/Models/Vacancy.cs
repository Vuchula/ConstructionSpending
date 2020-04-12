using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Vacancy
    {
        public int VacancyID { get; set; }
        public VacancyType VacancyType { get; set; }
        public decimal? Value { get; set; }

        public int TimeID { get; set; } //declares as not null
        public Time Time { get; set; }

        public int MarketID { get; set; }
        public Market Market { get; set; }
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
