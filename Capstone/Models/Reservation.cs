using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Reservation
    {
        public int reservation_id { get; set; }

        public string site_id { get; set; }

        public string name { get; set; }

        public DateTime from_date { get; set; }

        public DateTime to_date { get; set; }

        public DateTime create_date { get; set; }
        
    }
}
