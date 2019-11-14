using MySql.Data.MySqlClient;
using System;

namespace NTDOY_Microservice
{
    public class DB_Connection
    {
        public static MySqlConnection conn = null; //connection to db
        private static string connection_string = Environment.GetEnvironmentVariable("dbconnection");

        //attempt to connect to the database
        public static void Connect() {
            try
            {
                Console.WriteLine("TEST: " + connection_string);
                DB_Connection connection = new DB_Connection();
                conn = new MySqlConnection();
                conn.ConnectionString = connection_string;
                conn.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                conn = null;
            }
        }

        //used for testing, set the connection to something else
        //possibly a mock or test database
        public void SetConnection(MySqlConnection newConn)
        {
            conn = newConn;
        }
    }
}
