using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Site
    {


       public int site_id { get; set; }

        public int campground_id { get; set; }

        public int site_number { get; set; }

        public int max_occupancy { get; set; }

        public bool accessible { get; set; }

        public int max_rv_length { get; set; }

        public bool utilities { get; set; }

        

    }
}
