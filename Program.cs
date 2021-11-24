using System;
using System.IO;

namespace HabitTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbPath = @"C:\tutorials\HabitTracker\db.sqlite";

            if (!File.Exists(dbPath))
                File.Create(dbPath);

            GetUserInput();
        }

        static void GetUserInput()
        {
            string commandInput = Console.ReadLine();
            int command = Convert.ToInt32(commandInput);

            bool closeArea = false;
            while (closeArea == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");

                switch (command)
                {
                    case 0:
                        closeArea = true;
                        break;
                    case 1:
                        GetAllRecords();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 1.\n");
                        break;
                }
            }
        }
    }
}
