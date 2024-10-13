using System;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

class BillsChain
{
    static string connectionString = Configuration.connectionString; // "Data Source=KHOLOUDPC\\SQLEXPRESS;Initial Catalog=TerminalDatabase;Integrated Security=True;TrustServerCertificate=True;";


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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string queryLastRecord = "SELECT TOP 1 ID, BillInformation, PrinterBalance, Signature, (SELECT TOP 1 Signature FROM BillsChain WHERE ID < (SELECT MAX(ID) FROM BillsChain) ORDER BY ID DESC) as PreviousSignature FROM BillsChain ORDER BY ID DESC";

                using (SqlCommand command = new SqlCommand(queryLastRecord, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
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
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO BillsChain (BillInformation, PrinterBalance, Signature ,BalanceUpdated) VALUES (@BillInformation, @PrinterBalance, @Signature , 0)";
            using (SqlCommand command = new SqlCommand(query, connection))
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
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE BillsChain SET PrinterBalance = @PrinterBalance, Signature = @Signature  , BalanceUpdated=1  WHERE ID = @ID";
            using (SqlCommand command = new SqlCommand(query, connection))
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string querySelectbill = "SELECT TOP 1 ID FROM [TerminalDatabase].[dbo].[BillsChain] where [BillInformation] LIKE '%" + billNumber + "%'";

                using (SqlCommand command = new SqlCommand(querySelectbill, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
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
