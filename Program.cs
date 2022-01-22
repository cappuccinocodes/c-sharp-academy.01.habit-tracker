using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace HabitTracker
{
    class Program
    {
        static string dbPath = @"C:\Projects\Tutorials\HabitTracker\db.db";
        static string connectionString = @"Data Source=db.db";
        static void Main()
        {
            //string dbPath = @"C:\Projects\Tutorials\HabitTracker";

            if (!File.Exists(dbPath))
            {
                File.Create(dbPath);
            }

            CreateTable();

            GetUserInput();
        }

        static void GetUserInput()
        {

            bool closeArea = false;
            while (closeArea == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert Record.");
                Console.WriteLine("Type 3 to Delete Record.");
                Console.WriteLine("Type 4 to Update Record.");


                string commandInput = Console.ReadLine();

                while (string.IsNullOrEmpty(commandInput) || !int.TryParse(commandInput, out _))
                {
                    Console.WriteLine("\nCommand Invalid");
                    commandInput = Console.ReadLine();
                }

                int command = Convert.ToInt32(commandInput);

                switch (command)
                {
                    case 0:
                        closeArea = true;
                        break;
                    case 1:
                        GetAllRecords();
                        break;
                    case 2:
                        Insert();
                        break;
                    case 3:
                        Delete();
                        break;
                    case 4:
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;
                }
            }
        }

        internal static void CreateTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"CREATE TABLE IF NOT EXISTS drinking_water (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, 'Quantity' INTEGER ) ";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        internal static void GetAllRecords()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water ";

                List<DrinkingWater> tableData = new();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = ((double)reader.GetInt32(2) / 10)
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }
               
                connection.Close();

                foreach (DrinkingWater dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - {dw.Quantity}l");
                }

            }
        }

        internal static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetQuantityInput();

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText =
                       $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

                    tableCmd.ExecuteNonQuery();

                    connection.Close();
                }

                GetUserInput();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw; 
            }
        }

        internal static void Delete()
        {

            GetAllRecords();

            Console.WriteLine("\n\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu\n\n");

            string recordId = Console.ReadLine();

            if (recordId == "0") GetUserInput();

            while (!Int32.TryParse(recordId, out _) || string.IsNullOrEmpty(recordId))
            {
                Console.WriteLine("\n\nInvalid Id. Try again.\n\n");
                recordId = Console.ReadLine();
            }

            var Id = Int32.Parse(recordId);

            if (Id == 0) GetUserInput();

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = '{Id}'";
                    int rowCount = tableCmd.ExecuteNonQuery();

                    if (rowCount == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {Id} doesn't exist. \n\n");
                        Delete();
                    }
                }

                Console.WriteLine($"\n\nRecord with Id {Id} was deleted. \n\n");

                GetUserInput();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        internal static void Update()
        {
            GetAllRecords();

            Console.WriteLine("\n\nPlease type Id of the record would like to update. Type 0 to return to main manu.\n\n");

            string recordId = Console.ReadLine();

            if (recordId == "0") GetUserInput();

            while (!Int32.TryParse(recordId, out _) || string.IsNullOrEmpty(recordId))
            {
                Console.WriteLine("\n\nInvalid Id. Try again.\n\n");
                recordId = Console.ReadLine();
            }

            var Id = Int32.Parse(recordId);

            if (Id == 0) GetUserInput();

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {Id})";
                    int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {Id} doesn't exist.\n\n");
                        Update();
                    }

                    string date = GetDateInput();
                    int quantity = GetQuantityInput();

                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {Id}";
                    tableCmd.ExecuteNonQuery();

                    connection.Close();
                }
                GetUserInput();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main manu.\n\n");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main manu or try again:\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        internal static int GetQuantityInput()
        {
            Console.WriteLine("\n\nPlease insert the amount in liters. Type 0 to return to main manu.\n\n");

            string quantityInput = Console.ReadLine();

            if (quantityInput == "0") GetUserInput();

            while (!Double.TryParse(quantityInput, out _) || Convert.ToDouble(quantityInput) < 0)
            {
                Console.WriteLine("\n\nInvalid amount. Try again.\n\n");
                quantityInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(Convert.ToDouble(quantityInput) * 10);

            return finalInput;
        }
    }
    

    public class DrinkingWater
    {
        public int Id { get; set;  }
        public DateTime Date { get; set; }
        public int Quantity { get; set;  }
    }
}

