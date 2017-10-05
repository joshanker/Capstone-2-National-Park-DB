using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Campground
    {

        public int campground_id { get; set; }

        public string name { get; set; }

        public string park_id { get; set; }

        public int open_from_mm { get; set; }

        public int open_to_mm { get; set; }

        public double daily_fee { get; set; }

        


    }
}
