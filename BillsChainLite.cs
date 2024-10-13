using System;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using SQLitePCL;


using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

class BillsChainLite
{
    static string dbFilePath = Configuration.dbFilePath;
    static string STAmount = Configuration.STAmount;
    //    static string connectionString = $"Data Source={dbFilePath}";
    static string connectionString = $"Data Source={dbFilePath};Password={CalculateMD5Hash(STAmount)};";

    public static string CreateDatabase(string configFilePath)
        {
        // Check if the database file already exists
        if (!File.Exists(dbFilePath))
        {
            // Create the SQLite connection string
            //string connectionString = $"Data Source={dbFilePath}";

            // Initialize SQLitePCL
            Batteries.Init();

            if (dbFilePath == "")
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 10);
                DateTime now = DateTime.Now;
                string DatabaseFileName = "Rasheed" + randomNumber.ToString() + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".db";

                dbFilePath = Configuration.XMLFolder.Remove(Configuration.XMLFolder.Length-1) + DatabaseFileName;
                STAmount = Configuration.STAmount;
                connectionString = $"Data Source={dbFilePath};Password={CalculateMD5Hash(STAmount)};";

                // MessageBox.Show(dbFilePath);
                try
                {
                    XmlDocument xmlDoc1 = new XmlDocument();
                    using (XmlReader reader = XmlReader.Create(configFilePath))
                    {

                        xmlDoc1.Load(reader); // Load the XML file
                        XmlNode configNode = xmlDoc1.SelectSingleNode("Configuration");

                        if (configNode != null)
                        {
                            configNode.SelectSingleNode("DatabaseFile").InnerText = dbFilePath;
                        }


                    }

                    xmlDoc1.Save(configFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred 1: " + ex.StackTrace);
                  //  MessageBox.Show(ex.Message);
                    return "atabase Creation Error";
                }
                Configuration.dbFilePath = dbFilePath;
                
            }

            // Open the connection, which creates the database file if it doesn't exist
            using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                        // Optionally, create tables or add data here
                        string createTableQuery = @"CREATE TABLE BillsChain (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                            BillInformation TEXT,
                            PrinterBalance INTEGER,
                            Signature TEXT,
                            BalanceUpdated INTEGER
                        )";
                    using (var command = new SqliteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

             return "Database Created";
            }
            else
            {
            // Database file already exists

            return "Database already exists.";
            }
        }

    public static (int CurrentBalance , string CurrStatus ,string Signature)? GetCurrCredit()
    {
        var result = GetLastRecord();

        if (result.Success)
        {

            if (result.LastRecord.HasValue)
            {
                var lastRecord = result.LastRecord;
                string previousSignature = result.PreviousSignature ?? "";

                if (CheckSignature(previousSignature, lastRecord.Value.PrinterBalance, lastRecord.Value.BillInformation, lastRecord.Value.Signature))
                {
                    return (lastRecord.Value.PrinterBalance, "Signature_Correct", lastRecord.Value.Signature);
                }

                else
                    return (0, "Signature_NotCorrect", lastRecord.Value.Signature);
            }
            else
            {

                return (0, "No_Data", "");
            }

        }
        else
        {
            return (0, "Database_Error", "");
        }
        


    }
    public static bool UpdateCredit(int NewBalance, string NewBillInfo , string PrevSignature)
    {
        try
        {
            // Create a new signature
            string newSignature = CreateNewSignature(PrevSignature, NewBalance, NewBillInfo);
            // Insert a new row in the database
            InsertNewRow(NewBillInfo, NewBalance, newSignature);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }



    }

    static string CalculateMD5Hash(string rawData)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    static bool CheckSignature(string previousSignature, decimal newBalance, string billInformation, string signature)
    {
        string rawData = previousSignature + newBalance.ToString() + billInformation;
        string calculatedSignature = CalculateMD5Hash(rawData);

        return calculatedSignature == signature;
    }


    static string CreateNewSignature(string previousSignature, decimal newBalance, string billInformation)
    {
        string rawData = previousSignature + newBalance.ToString() + billInformation;
        return CalculateMD5Hash(rawData);
    }

    static ((int ID, string BillInformation, int PrinterBalance, string Signature)? LastRecord, string? PreviousSignature, bool Success, string ErrorMessage) GetLastRecord()
    {

        try
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string queryLastRecord = " SELECT ID, BillInformation, PrinterBalance,  Signature, (SELECT Signature FROM BillsChain WHERE ID < (SELECT MAX(ID) FROM BillsChain) ORDER BY ID DESC LIMIT 1) as PreviousSignature  FROM BillsChain ORDER BY ID DESC   LIMIT 1";

                using (SqliteCommand command = new SqliteCommand(queryLastRecord, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string billInformation = reader.GetString(1);
                            int printerBalance = reader.GetInt32(2);
                            string lastSignature = reader.GetString(3);
                            string? previousSignature = reader.IsDBNull(4) ? (string?)null : reader.GetString(4);



                            return ((id, billInformation, printerBalance, lastSignature), previousSignature,true, "");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return (null, null, false, ex.Message);
        }
        return (null, null, true, "No records found");
    }

    static void InsertNewRow(string billInformation, decimal printerBalance, string signature)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO BillsChain (BillInformation, PrinterBalance, Signature ,BalanceUpdated) VALUES (@BillInformation, @PrinterBalance, @Signature , 0)";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillInformation", billInformation);
                command.Parameters.AddWithValue("@PrinterBalance", printerBalance);
                command.Parameters.AddWithValue("@Signature", signature);
                command.ExecuteNonQuery();
            }
        }
    }


    public static bool UpdateNewBalance(int NewBalance)
    {
        var result = GetLastRecord();


        if (!result.Success)
        {
            return false;
        }
        var lastRecord = result.LastRecord;

        if (!lastRecord.HasValue)
            return false;
        string previousSignature = result.PreviousSignature ?? "";
        int RecID = lastRecord.Value.ID;

        bool isSignatureValid = CheckSignature(previousSignature, lastRecord.Value.PrinterBalance, lastRecord.Value.BillInformation, lastRecord.Value.Signature);

        if (!isSignatureValid)
        {

            Console.WriteLine("Signature is not correct.");
            return false;
        }

        decimal updatedBalance = lastRecord.Value.PrinterBalance + NewBalance;

        // Create a new signature
        string newSignature = CreateNewSignature(previousSignature, updatedBalance, lastRecord.Value.BillInformation);

        // Update the record in the database
        UpdateRow(lastRecord.Value.ID, updatedBalance, newSignature);



        return true;
    }
    static void UpdateRow(int ID , decimal printerBalance, string signature)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE BillsChain SET PrinterBalance = @PrinterBalance, Signature = @Signature  , BalanceUpdated=1  WHERE ID = @ID";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@PrinterBalance", printerBalance);
                command.Parameters.AddWithValue("@Signature", signature);
                command.ExecuteNonQuery();
            }
        }
    }

    public static bool CheckIfPrintedBefore(string billNumber)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string querySelectbill = " SELECT ID  FROM BillsChain   WHERE BillInformation LIKE '%' || @billNumber || '%'  ORDER BY ID DESC   LIMIT 1";

                using (SqliteCommand command = new SqliteCommand(querySelectbill, connection))
                {
                    command.Parameters.AddWithValue("@billNumber", billNumber);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            if (id != 0)
                                return true;
                            else
                                return false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return false;
    }
}
