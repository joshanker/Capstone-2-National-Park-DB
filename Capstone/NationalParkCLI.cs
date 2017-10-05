using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;

//MAIN MENU
//1.  Show a list of all parks.
//2.  View campgrounds and make a reserveration for your park

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
        const string Command_ReturnToPreviousScreen = "3";
        const string Command_SearchForReservation = "2";
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


                Console.WriteLine(p.Id);
                Console.WriteLine(p.name);
                Console.WriteLine(p.location);
                Console.WriteLine(p.establishdate);
                Console.WriteLine(p.area);
                Console.WriteLine(p.visitors);
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
            Console.WriteLine("2.  View campgrounds and make a reserveration for your park");
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
            Console.WriteLine("2.  Search for reservation");
            Console.WriteLine("3.  Return to previous screen");

            string ParkMenuChoice = CLIHelper.GetString("What option would you like?");

            switch (ParkMenuChoice.ToLower())
            {
                case Command_ViewCampgrounds:
                    ShowAllCampgrounds(Id);
                    break;

                case Command_SearchForReservation:
                    ViewReservations(Id);
                    break;

                case Command_ReturnToPreviousScreen:
                    break;

            }

            
        }


        private List<Campgrounds> ShowAllCampgrounds(int ParkID)
        {

        }




    }
}
