using MySql.Data.MySqlClient;
using System;

namespace NTDOY_Microservice.Models
{
    //data storage for Buy and Sell database operations
    public class BuySell
    {
        public string Username { get; set; }
        public string Account { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }

        //save a buy to the database and return the ID for final verification in central server
        public int LogBuy()
        {
            string query = "Insert into buy_sell (b_type, username, t_account, price, quantity) " +
                           "Values (\"BUY\",\"" + Username + "\",\"" + Account + "\"," + Price + "," + Quantity + ")";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = query;
                comm.ExecuteNonQuery();
                return (int)comm.LastInsertedId;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        //save a sell to the database and return the ID for final verification in central server
        public int LogSell()
        {
            string query = "Insert into buy_sell (b_type, username, t_account, price, quantity) " +
                           "Values (\"SELL\",\"" + Username + "\",\"" + Account + "\"," + Price + "," + Quantity + ")";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = query;
                comm.ExecuteNonQuery();
                return (int)comm.LastInsertedId;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        //get the current stock ammount for the bank
        public static int BankStock()
        {
            string sql = "select stocks_owned(\"admin\", \"OBS\") as stocks";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = sql;
                MySqlDataReader rdr = comm.ExecuteReader();
                rdr.Read(); //advance to the response
                int num = (int)rdr["stocks"];
                rdr.Close();
                return num;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.StackTrace);
                return -1;
            }
        }

        //purchase stock for the bank
        //returns false if it failed
        public static bool BankPurchase(int purchase, float price)
        {
            string sql = "Insert into buy_sell (b_type, username, t_account, price, quantity)" +
                " values (\"BUY\",\"admin\",\"OBS\"," + price + "," + purchase + ")";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = sql;
                comm.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        //gets the currecnt stock ammount for this users account
        public int UserAccountStock()
        {
            string sql = "select stocks_owned(\"" + Username + "\",\"" + Account + "\") as stocks";
            try
            {
                MySqlCommand comm = DB_Connection.conn.CreateCommand();
                comm.CommandText = sql;
                MySqlDataReader rdr = comm.ExecuteReader();
                rdr.Read(); //advance to the response
                int num = (int)rdr["stocks"];
                rdr.Close();
                return num;
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.StackTrace);
                return -1;
            }
        }
    }

}
