using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class NationalParkDAL
    {

        private string connectionString;
        const string SQL_ShowAllParks = "select * from park";
        const string SQL_ShowParkDetail = "select * from park where park_id = @Id";

        public NationalParkDAL(string connection)
        {
           connectionString = connection;
        }

        public List<Park> ShowAllParks()
        {
            List<Park> parklist = new List<Park>();
            //read teh DB and assemble a park object
            // add park object to the list.
            //return the list.

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(SQL_ShowAllParks, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Park p = GetParkFromReader(reader);
                        parklist.Add(p);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return parklist;

        }

        private static Park GetParkFromReader(SqlDataReader reader)
        {
            Park p = new Park();

            p.Id = Convert.ToInt32(reader["park_id"]);
            p.name = Convert.ToString(reader["name"]);
            p.location = Convert.ToString(reader["location"]);
            p.establishdate = Convert.ToDateTime(reader["establish_date"]);
            p.area = Convert.ToInt32(reader["area"]);
            p.visitors = Convert.ToInt32(reader["visitors"]);
            p.description = Convert.ToString(reader["description"]);
            return p;
        }

        public Park GetParkInfo(int parkToReturn)
        {
            Park loadedUpPark = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(SQL_ShowParkDetail, connection);
                    cmd.Parameters.AddWithValue("@Id", parkToReturn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        loadedUpPark = GetParkFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return loadedUpPark;            
        }


    



    }
}
