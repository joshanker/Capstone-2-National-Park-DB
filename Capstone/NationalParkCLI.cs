using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;
using System.Configuration;

//MAIN MENU
//1.  Show a list of all parks.
//2.  View campgrounds and make a reservation for your park

//Sub Menu for chosen park:

//1.  Show a list of campgrounds for a park #
//2.  select a CG and search for date availability.
//3.  book a reservation.

namespace Capstone
{
    public class NationalParkCLI
    {
        readonly string DatabaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        const string Command_ViewAllParks = "1";
        const string Command_ViewParkDetails = "2";
        //const string Command_MakeAReservation = "3";
        const string Command_ReturnToPreviousScreen = "3";
        const string Command_SearchForAvailableReservations = "2";
        const string Command_ViewCampgrounds = "1";
        const string Command_Quit = "q";

        public void run()
        {
            PrintHeader();
            PrintMenu();


            while (true)
            {
                string command = Console.ReadLine();

                switch (command.ToLower())
                {
                    case Command_ViewAllParks:
                        ShowAllParks();
                        break;

                    case Command_ViewParkDetails:
                        ViewParkDetails();
                        break;

                    case Command_Quit:
                        Quit();
                        break;

                }

                PrintHeader();
                PrintMenu();

            }
        }

        private void ShowAllParks()
        {
            Console.WriteLine();
            NationalParkDAL npDAL = new NationalParkDAL(DatabaseConnection);
            List<Park> parkList = npDAL.ShowAllParks();


            if (parkList.Count > 0)
            {
                foreach (Park p in parkList)
                {
                    Console.Write(p.Id);
                    Console.Write(".  ");
                    Console.WriteLine(p.name);
                }
            }
            else
            {
                Console.WriteLine("NO RESULTS IN PARKLIST");
            }

        }

        private void ViewParkDetails()
        {

            int parkToView = CLIHelper.GetInteger("What park would you like to view?");
            Console.WriteLine();
            NationalParkDAL npDAL = new NationalParkDAL(DatabaseConnection);

            Park p = npDAL.GetParkInfo(parkToView);


            if (p != null)
            {
                Console.WriteLine("Park ID: " + p.Id);
                Console.WriteLine("Park Name: " + p.name);
                Console.WriteLine("Location: " + p.location);
                Console.WriteLine("Established: " + p.establishdate);
                Console.WriteLine("Area: " + p.area);
                Console.WriteLine("Visitors " + p.visitors);
                Console.WriteLine(p.description);

                ParkSubmenu(p.Id);
            }
            else
            {
                Console.WriteLine("NO RESULTS IN PARKLIST");
            }
        }





        private void PrintMenu()
        {
            Console.WriteLine("1.  Show all parks");
            Console.WriteLine("2.  View park submenu and make reservations.");
            Console.WriteLine("q   Quits the application.");
        }

        private void PrintHeader()
        {
            Console.WriteLine("--------------------------------------------------------------");

            Console.WriteLine("NATIONAL PARK DB");


            Console.WriteLine("--------------------------------------------------------------");
        }

        private void Quit()
        {
            Environment.Exit(1);
        }

        private void ParkSubmenu(int id)
        {

            Console.WriteLine("1.  View Campgrounds");
            Console.WriteLine("2.  Search for and make a reservation");
            //Console.WriteLine("3.  Make a reservation");
            Console.WriteLine("3.  Return to previous screen");

            string parkMenuChoice = CLIHelper.GetString("What option would you like?");

            switch (parkMenuChoice.ToLower())
            {
                case Command_ViewCampgrounds:
                    ShowAllCampgrounds(id);
                    break;

                case Command_SearchForAvailableReservations:
                    SearchForAvailableReservations(id);
                    break;

                //case Command_MakeAReservation:
                //    MakeReservation(Id);
                //    break;

                case Command_ReturnToPreviousScreen:
                    break;

            }


        }


        private void ShowAllCampgrounds(int parkId)
        {
            CampgroundDAL cDAL = new CampgroundDAL(DatabaseConnection);
            List<Campground> clist = new List<Campground>();
            clist = cDAL.ShowAllCampgrounds(parkId);


            if (clist.Count > 0)
            {

                Console.WriteLine("CG ID ".PadRight(5) + "Name".PadRight(35) + "Open From".PadRight(14) + "Open To".PadRight(10) + "Daily Fee".PadRight(10));
                foreach (Campground c in clist)
                {

                    Console.Write("  " + c.CampgroundId.ToString().PadRight(3) + " " + c.Name.ToString().PadRight(30) + "        " + c.OpenFromMM.ToString().PadRight(10) + " " + c.OpenToMM.ToString().PadRight(10) + " " + c.DailyFee);
                    Console.WriteLine();

                }

            }

            else
            {
                Console.WriteLine("NO RESULTS IN CAMPGROUND LIST");
            }


        }

        private void SearchForAvailableReservations(int ParkID)
        {

            string CampgroundChoice = CLIHelper.GetString("Which campground would you like?");
            string ArrivalDate = CLIHelper.GetDate("What is the arrival date?");
            string DepartureDate = CLIHelper.GetDate("What is the departure date?");

            // This needs to return a list of available sites (for each campground) for those dates.

            SiteDAL sdal = new SiteDAL(DatabaseConnection);

            List<Site> slist = sdal.ViewAvailReservations(CampgroundChoice, ArrivalDate, DepartureDate);



            
            if (slist.Count > 0)
            {

                CampgroundDAL cgdal = new CampgroundDAL(DatabaseConnection);

                double camgroundCost = cgdal.GetCampgroundCost(CampgroundChoice);

                Console.WriteLine("Available sites & details for your dates:");
                Console.WriteLine("Site ID" + " " + "Max Occupancy" + " " + "Accessible?" + " " + "Max RV Length" + " " + "Utilities" + " " + "Cost");

                foreach (Site item in slist)
                {
                    Console.Write("   " + item.site_id.ToString().PadRight(9) + " " + item.max_occupancy.ToString().PadRight(10) + " " + TrueFalse(item.accessible).ToString().PadRight(14) + " " + item.max_rv_length.ToString().PadRight(10) + " " + TrueFalse(item.utilities).ToString().PadRight(8) + " " + camgroundCost);
                    Console.WriteLine();
                }


                string siteChoiceToReserve = CLIHelper.GetString("Which site should be reserved (enter 0 to cancel)");

                if (siteChoiceToReserve == "0")
                {
                    return;
                }


                bool SiteIsInTheList = false;



                foreach (Site item in slist)
                {
                    if (item.site_id.ToString() == siteChoiceToReserve)
                    {
                        SiteIsInTheList = true;
                    }
                }

                if (!SiteIsInTheList)

                {
                    Console.WriteLine("Sorry, that Site ID isn't in our list!  Please start over.");
                    return;
                }



                string name = CLIHelper.GetString("What name should the reservation be made under?");
                MakeReservation(siteChoiceToReserve, name, ArrivalDate, DepartureDate);
            }
            else
            {
                Console.WriteLine("Sorry, no campsites are available. Please try again with different dates.");
                return;
            }
        }

        private void MakeReservation( string CampSiteChoice, string name, string ArrivalDate, string DepartureDate)
        {

            //string CampgroundChoice = CLIHelper.GetString("Which campground would you like?");
            //string CampSiteChoice = CLIHelper.GetString("Which campsite would you like?");
            //string name = CLIHelper.GetString("What is your name?");
            //string ArrivalDate = CLIHelper.GetString("What is the arrival date?");
            //string DepartureDate = CLIHelper.GetString("What is the departure date?");

            ReservationDAL rDal = new ReservationDAL(DatabaseConnection);
            int confirmationNumber = rDal.AddReservation(CampSiteChoice, name, ArrivalDate, DepartureDate);
            if (confirmationNumber == 0)
            {
                Console.WriteLine("There was an error during the reservation");

            }
            else
            {
                Console.WriteLine($"Your confirmation number is {confirmationNumber}");
            }

        }

        //private void MakeReservation(int Id)
        //{

        //    string CampgroundChoice = CLIHelper.GetString("Which campground would you like?");
        //    string CampSiteChoice = CLIHelper.GetString("Which campsite would you like?");
        //    string name = CLIHelper.GetString("What is your name?");
        //    string ArrivalDate = CLIHelper.GetString("What is the arrival date?");
        //    string DepartureDate = CLIHelper.GetString("What is the departure date?");

        //    ReservationDAL rDal = new ReservationDAL(DatabaseConnection);
        //    int confirmationNumber = rDal.AddReservation(Id, CampgroundChoice, CampSiteChoice, name, ArrivalDate, DepartureDate);
        //    if (confirmationNumber == 0)
        //    {
        //        Console.WriteLine("There was an error during the reservation");

        //    }
        //    else
        //    {
        //        Console.WriteLine($"Your confirmation number is {confirmationNumber}");
        //    }

        //}

        private String TrueFalse(bool result)
        {

            if (result)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }

        }



    }
}








