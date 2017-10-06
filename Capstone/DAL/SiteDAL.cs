using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using System.Data.SqlClient;



namespace Capstone.DAL
{
    public class SiteDAL
    {

        private string connectionString;
        private const string SQL_ReturnAvailSitesForAGivenTime = 
            "SELECT TOP 5  * FROM site " +
            "WHERE site.campground_id = @campground AND site.site_id NOT IN (SELECT reservation.site_id FROM reservation WHERE ((@departureDate > reservation.from_date AND @departureDate <= reservation.to_date) OR (@arriveDate >= reservation.from_date AND @arriveDate < reservation.to_date) OR (@arriveDate >= reservation.from_date AND @arriveDate < reservation.to_date)))";

        

        //private string connectionString;
        //private const string SQL_ReturnAvailSitesForAGivenTime =
        //    "SELECT TOP 5  * FROM site " +
        //    "WHERE site.campground_id = @campground AND site.site_id NOT IN (SELECT reservation.site_id FROM reservation WHERE ((@arriveDate<reservation.from_date AND @departureDate> reservation.from_date AND @departureDate<reservation.to_date ) OR (@arriveDate > reservation.from_date AND @departureDate<=reservation.to_date) OR (@arriveDate > reservation.from_date AND @departureDate > reservation.to_date) OR (@arriveDate<reservation.from_date AND @departureDate> reservation.to_date) or (@arriveDate = reservation.from_date AND @departureDate = reservation.to_date)))";






        public SiteDAL(string connection)
        {
            connectionString = connection;
        }

        public List<Site> ViewAvailReservations(String Campground, String ArrivalDate, String DepartureDate)
        {
            List<Site> output = new List<Site>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(SQL_ReturnAvailSitesForAGivenTime, connection);
                    cmd.Parameters.AddWithValue("@arriveDate", DateTime.Parse(ArrivalDate));
                    cmd.Parameters.AddWithValue("@departureDate", DateTime.Parse(DepartureDate));
                    cmd.Parameters.AddWithValue("@campground", Convert.ToInt32(Campground));

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Site s = new Site();
                        Campground c = new Campground();

                        s.campground_id = Convert.ToInt32(reader["campground_id"]);
                        s.site_id = Convert.ToInt32(reader["site_id"]);
                        s.max_occupancy = Convert.ToInt32(reader["max_occupancy"]);
                        s.accessible = Convert.ToBoolean(reader["accessible"]);
                        s.max_rv_length = Convert.ToInt32(reader["max_rv_length"]);
                        s.utilities = Convert.ToBoolean(reader["utilities"]);
                        

                        output.Add(s);

                        
                    }

                    


                }


            }




            catch (SqlException)
            {
                throw;
            }

            return output;
        }


    }
}
