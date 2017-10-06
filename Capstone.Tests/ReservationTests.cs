using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.DAL;
using Capstone.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Capstone.Tests
{
    [TestClass]
    public class ReservationTests
    {
        TransactionScope t;
        const string DatabaseConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog = NationalParkDB;User ID = te_student;Password=sqlserver1";


        private const string SQL_InsertParkData = "INSERT INTO park (name, location, establish_date, area, visitors, description) VALUES('JaciTestPark', 'Ohio', '1979-06-25', 4134, 666, 'This is a test park'); SELECT CAST (Scope_identity() as int)";
        private int parkId;
        private const string SQL_InsertCampgroundData = "INSERT INTO campground (park_id, name, open_from_mm, open_to_mm, daily_fee) VALUES (@park_id, 'cg1', 1, 12, 25.00); SELECT CAST (Scope_identity() as int)";
        private int campgroundID;
        private int maxSiteNumberThatAlreadyExists;
        private const string SQL_GetMaxSiteNum = "select Max(site_id) from site";


        private const string SQL_InsertSiteData = "INSERT INTO site(site_number, campground_id, accessible, utilities) VALUES(1, @campground, 1, 1); SELECT CAST (Scope_identity() as int)";
        private int SiteID;





        [TestInitialize]
        public void TestInitialize()
        {
            t = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(DatabaseConnection))
            {
                connection.Open();

                //INSERT PARK DATA
                SqlCommand cmd = new SqlCommand(SQL_InsertParkData, connection);
                parkId = Convert.ToInt32(cmd.ExecuteScalar());

                //INSERT CAMPGROUND DATA
                cmd = new SqlCommand(SQL_InsertCampgroundData, connection);
                cmd.Parameters.AddWithValue("@park_id", parkId);
                campgroundID = Convert.ToInt32(cmd.ExecuteScalar());

                //INSERT SITE DATA
                //get max site # that already exists
                //cmd = new SqlCommand(SQL_GetMaxSiteNum, connection);
                //maxSiteNumberThatAlreadyExists = Convert.ToInt32(cmd.ExecuteScalar());
                //int newMaxSiteNumber = maxSiteNumberThatAlreadyExists++;

                cmd = new SqlCommand(SQL_InsertSiteData, connection);
                cmd.Parameters.AddWithValue("@campground", campgroundID);
                SiteID = Convert.ToInt32(cmd.ExecuteScalar());

                //make some more SQL entries to enter in reservations as needed.

            }

        }

        [TestCleanup]
        public void Cleanup()
        {
            t.Dispose();
        }

        [TestMethod]
        public void Test_NoReservations_ReturnsOpenSiteId()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now;
            DateTime toDate = DateTime.Now.AddDays(5);

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(campgroundID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(1, availableSites.Count);
            Assert.AreEqual(SiteID, availableSites[0].site_id);
        }



        [TestMethod]
        public void TestThatOurParkWasInsertedCorrectly()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConnection))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("Select name from park where park_id = @park_id", connection);
                    cmd.Parameters.AddWithValue("@park_id", parkId);

                    string assertNameOfPark = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("JaciTestPark", assertNameOfPark);
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        [TestMethod]
        public void TestThatOurCampgroundWasInsertedCorrectly()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConnection))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("Select name from campground where park_id = @park_id", connection);
                    cmd.Parameters.AddWithValue("@park_id", parkId);

                    string assertNameOfCampground = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("cg1", assertNameOfCampground);

                }

            }
            catch (SqlException)
            {
                throw;
            }
        }


        [TestMethod]
        public void TestThatOurCampgroundIsIDScalar()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConnection))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("Select campground_id from campground where park_id = @park_id", connection);
                    cmd.Parameters.AddWithValue("@park_id", parkId);

                    int assertNumOfCampground = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(campgroundID, assertNumOfCampground);

                }

            }
            catch (SqlException)
            {
                throw;
            }
        }


        [TestMethod]
        public void TestThatOurSiteWasInsertedCorrectly()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConnection))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("select site_id from site where campground_id = @campgroundID", connection);

                    cmd.Parameters.AddWithValue("@campgroundID", campgroundID);

                    int assertNumberOfSite = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(SiteID, assertNumberOfSite);

                }

            }
            catch (SqlException)
            {
                throw;
            }
        }

        


        [TestMethod]
        public void RequestSitesBeforeASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(1);
            DateTime toDate = DateTime.Now.AddDays(4);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(5);
            DateTime alreadyInToDate = DateTime.Now.AddDays(10);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation("1", "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(campgroundID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(1, availableSites.Count);
            Assert.AreEqual(SiteID, availableSites[0].site_id);
        }


        [TestMethod]
        public void RequestSitesAfterASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(5);
            DateTime toDate = DateTime.Now.AddDays(10);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(1);
            DateTime alreadyInToDate = DateTime.Now.AddDays(4);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation("1", "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(campgroundID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(1, availableSites.Count);
            Assert.AreEqual(SiteID, availableSites[0].site_id);
        }



        [TestMethod]
        public void RequestSitesThatOverlapTheStartOfASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(3);
            DateTime toDate = DateTime.Now.AddDays(10);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(1);
            DateTime alreadyInToDate = DateTime.Now.AddDays(4);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation(campgroundID.ToString(), "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(SiteID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(0, availableSites.Count);
            
        }

        [TestMethod]
        public void RequestSitesThatOverlapTheEndOfASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(2);
            DateTime toDate = DateTime.Now.AddDays(6);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(1);
            DateTime alreadyInToDate = DateTime.Now.AddDays(4);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation(campgroundID.ToString(), "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(SiteID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(0, availableSites.Count);
           
        }



        [TestMethod]
        public void RequestSiteWhenMyStayIsWiderThanASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(1);
            DateTime toDate = DateTime.Now.AddDays(6);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(2);
            DateTime alreadyInToDate = DateTime.Now.AddDays(4);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation(campgroundID.ToString(), "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(SiteID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(0, availableSites.Count);

        }


        [TestMethod]
        public void RequestSiteWhenMyStayIsInsideASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(3);
            DateTime toDate = DateTime.Now.AddDays(6);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(2);
            DateTime alreadyInToDate = DateTime.Now.AddDays(8);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation(campgroundID.ToString(), "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(SiteID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(0, availableSites.Count);

        }


        [TestMethod]
        public void RequestSiteWhenMyStayIsExactlyTheSameAsASavedReservation()
        {
            //Arrange
            SiteDAL dal = new SiteDAL(DatabaseConnection);
            DateTime fromDate = DateTime.Now.AddDays(2);
            DateTime toDate = DateTime.Now.AddDays(8);

            DateTime alreadyInFromDate = DateTime.Now.AddDays(2);
            DateTime alreadyInToDate = DateTime.Now.AddDays(8);

            ReservationDAL rdal = new ReservationDAL(DatabaseConnection);
            rdal.AddReservation(campgroundID.ToString(), "TestFamily", alreadyInFromDate.ToString(), alreadyInToDate.ToString());

            //Act
            List<Site> availableSites = dal.ViewAvailReservations(SiteID.ToString(), fromDate.ToString(), toDate.ToString());

            //Assert
            Assert.AreEqual(0, availableSites.Count);

        }






    }
}

