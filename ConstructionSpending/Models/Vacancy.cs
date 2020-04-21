using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Vacancy
    {
        public int VacancyID { get; set; }
        public VacancyType VacancyType { get; set; }
        public double? Value { get; set; }
        public UnitOfMeasure UoM { get; set; }
        public bool SeasonallyAdjusted { get; set; }
        [Required]
        public Time Time { get; set; }
        
        //[ForeignKey("Market")]
        public int? MarketID { get; set; } 
        public Market Market { get; set; }
    }

    public enum VacancyType
    {
        Year_Round,
        Seasonal,
        Total_Vacant
    }

    public class Market
    {
        public int MarketID { get; set; }
        public MarketStatus MarketStatus { get; set; }
        public MarketType? MarketType { get; set; }
        public bool? On_Contract { get; set; }
        public HeldOffType? HeldOffType { get; set; }
        [Required]
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
