using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;

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
        const string DatabaseConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog = NationalParkDB;User ID = te_student;Password=sqlserver1";
        const string Command_ViewAllParks = "1";
        const string Command_ViewParkDetails = "2";
        const string Command_MakeAReservation = "3";
        const string Command_ReturnToPreviousScreen = "4";
        const string Command_SearchForAvailableReservations = "2";
        const string Command_ViewCampgrounds = "1";
        const string Command_Quit = "q";
        List<Park> GlobalListOfParks;

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
                        quit();
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
            GlobalListOfParks = parkList;


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

        private void quit()
        {
            Environment.Exit(1);
        }

        private void ParkSubmenu(int Id)
        {

            Console.WriteLine("1.  View Campgrounds");
            Console.WriteLine("2.  Search for Available reservations");
            Console.WriteLine("3.  Make a reservation");
            Console.WriteLine("4.  Return to previous screen");

            string ParkMenuChoice = CLIHelper.GetString("What option would you like?");

            switch (ParkMenuChoice.ToLower())
            {
                case Command_ViewCampgrounds:
                    ShowAllCampgrounds(Id);
                    break;

                case Command_SearchForAvailableReservations:
                    SearchForAvailableReservations(Id);
                    break;

                case Command_MakeAReservation:
                    MakeReservation(Id);
                    break;

                case Command_ReturnToPreviousScreen:
                    break;

            }


        }


        private void ShowAllCampgrounds(int ParkID)
        {
            CampgroundDAL cDAL = new CampgroundDAL(DatabaseConnection);
            List<Campground> clist = new List<Campground>();
            clist = cDAL.ShowAllCampgrounds(ParkID);


            if (clist.Count > 0)
            {

                Console.WriteLine("CG ID ".PadRight(5) + "Name".PadRight(35) + "Open From".PadRight(14) + "Open To".PadRight(10) + "Daily Fee".PadRight(10));
                foreach (Campground c in clist)
                {

                    Console.Write("  " + c.campground_id.ToString().PadRight(3) + " " + c.name.ToString().PadRight(30) + "        " + c.open_from_mm.ToString().PadRight(10) + " " + c.open_to_mm.ToString().PadRight(10) + " " + c.daily_fee);
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
            string ArrivalDate = CLIHelper.GetString("What is the arrival date?");
            string DepartureDate = CLIHelper.GetString("What is the departure date?");


        //    List<Reservation> rlist = ViewAvailReservations(ParkID, CampgroundChoice, ArrivalDate, DepartureDate);

        }

        private void MakeReservation(int Id)
        {
           
            string CampgroundChoice = CLIHelper.GetString("Which campground would you like?");
            string CampSiteChoice = CLIHelper.GetString("Which campsite would you like?");
            string name = CLIHelper.GetString("What is your name?");
            string ArrivalDate = CLIHelper.GetString("What is the arrival date?");
            string DepartureDate = CLIHelper.GetString("What is the departure date?");

            ReservationDAL rDal = new ReservationDAL(DatabaseConnection);
            int confirmationNumber = rDal.AddReservation(Id, CampgroundChoice,  CampSiteChoice, name, ArrivalDate, DepartureDate);
            if(confirmationNumber == 0)
            {
                Console.WriteLine("There was an error during the reservation");

            }
            else
            {
                Console.WriteLine($"Your confirmation number is {confirmationNumber}");
            }

        }

    }
}








