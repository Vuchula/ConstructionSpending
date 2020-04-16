using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionSpending.Models
{
    public class Response
    {
        public string data_type_code { get; set; }
        public string cell_value { get; set; }
        public string program_code { get; set; }
        public string time_slot_date { get; set; }
        public string seasonally_adj { get; set; }
        public string time_slot_id { get; set; }
        public string time_slot_name { get; set; }
        public string category_code { get; set; }
        public string time { get; set; }


    }
}


