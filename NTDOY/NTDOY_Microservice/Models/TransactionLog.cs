using System;
using MySql.Data.MySqlClient;
using System.Collections;

namespace NTDOY_Microservice.Models
{
    //model of database entry for a log of a single request to an api endpoint
    //gets logged by middleware after request finishes
    public class TransactionLog
    {
        public string Origin { get; set; }  //must be set
        public string RequestType { get; set; } //must be set
        public string Type { get; set; }    //must be set
        public string Username { get; set; }    //might be unknown
        public string Account { get; set; } //might be unkown/unecessary
        public float Price { get; set; }    //might be unecessary
        public int Quantity { get; set; }   //might be unecessary

        //initialize values so they dont need to be manually set if they arent needed
        public TransactionLog()
        {
            Username = "";
            Account = "";
            Price = -1f;
            Quantity = -1;
        }

        //logs the information currently in this object to the database
        public void LogToDatabase()
        {
            string query = "Insert into transactions (origin, req_type, t_type, t_account, t_price, t_quantity, username) " +
                           "Values (\"" + Origin + "\",\"" + RequestType + "\",\"" + Type + "\",\"" + Account + "\"," + 
                           Price + "," + Quantity + ",\"" + Username + "\")";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = query;
                comm.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //select all transaction logs from the database
        public static ArrayList GetAllTransactions()
        {
            ArrayList cols = new ArrayList();   //array list to store all columns

            string sql = "SELECT * FROM transactions";
            MySqlCommand cmd = DB_Connection.conn.CreateCommand();
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cols.Add(new
                {
                    Id = rdr["t_id"],
                    Origin = rdr["origin"],
                    RequestType = rdr["req_type"],
                    DateCreated = rdr["t_time"],
                    Type = rdr["t_type"],
                    Username = rdr["username"],
                    Account = rdr["t_account"],
                    Price = rdr["t_price"],
                    Quantity = rdr["t_quantity"]
                });
            }

            rdr.Close();
            return cols;

        }

        //gets a stock quoute json response string and gets the last price value from it
        public static float GetPriceFromJson(string json)
        {
            int indexOfLast = json.IndexOf("last");     //find index of variable we want to get from response
            int indexOfColon = json.IndexOf(':', indexOfLast);  //find start of the price
            string price = json.Substring(indexOfColon + 1, json.IndexOf(',', indexOfLast) - indexOfColon - 1);
            return float.Parse(price);
        }
    }
}
