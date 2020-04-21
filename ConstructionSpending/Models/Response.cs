using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Response
    {
        public int ResponseId { get; set; }
        public string data_type_code { get; set; }
        public double cell_value { get; set; }
        public string program_code { get; set; }
        public DateTime time_slot_date { get; set; }
        public string seasonally_adj { get; set; }
        public int time_slot_id { get; set; }
        public string time_slot_name { get; set; }
        public string category_code { get; set; }
        public string time { get; set; }
        public string geo_level_code { get; set; }


    }
}


