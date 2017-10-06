using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundDAL
    {
        int campgroundId;
        private string connectionString;
        private const string SQL_ReturnCampgroundsForAPark = "select * from campground where park_id = @parkID";
        private const string SQL_ReturnCampgroundCost = "select daily_fee from campground where campground_id = @campgroundchoice";
        private double cGroundCost;


        public CampgroundDAL(string connection)
        {
            connectionString = connection;
        }


	    public List<Campground> ShowAllCampgrounds(int Id)
        {

            List<Campground> cglist = new List<Campground>();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(SQL_ReturnCampgroundsForAPark, connection);
                    cmd.Parameters.AddWithValue("@parkID", Id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        
                        Campground c = new Campground();

                        c.campground_id = Convert.ToInt32(reader["campground_id"]);
						c.park_id = Convert.ToInt32(reader["park_id"]);
                        c.name = Convert.ToString(reader["name"]);
						c.open_from_mm = Convert.ToInt32(reader["open_from_mm"]);
                        c.open_to_mm = Convert.ToInt32(reader["open_to_mm"]);
						c.daily_fee = Convert.ToDouble(reader["daily_fee"]);

                        cglist.Add(c);

                    }

                }

            }

            catch (SqlException ex)
            {
                throw;
            }

            return cglist;

        }

        public double GetCampgroundCost(string cgchoice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(SQL_ReturnCampgroundCost, connection);
                    cmd.Parameters.AddWithValue("@campgroundchoice", int.Parse(cgchoice));
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cGroundCost = Convert.ToDouble(reader["daily_fee"]);
                       

                    }

                }

            }

            catch (SqlException ex)
            {
                throw;
            }

            return cGroundCost;

        }

       

    }
}
