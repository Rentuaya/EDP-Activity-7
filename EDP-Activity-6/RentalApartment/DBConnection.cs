using MySql.Data.MySqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentalApartment
{
    public class DBConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnection(string server, string database, string uid, string password) 
        {
            this.server = server;
            this.database = database;
            this.uid = uid; 
            this.password = password;
            Initialize();
        }

        //Initialize Connections
        private void Initialize()
        {
            string connectionString = $"SERVER=localhost;DATABASE=apartment;UID=root;PASSWORD=rentuaya2022;";
            connection = new MySqlConnection(connectionString);
        }

        //Open Connection to Database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }catch (MySqlException ex)
            {
                //Handle Error
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //Close Connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close(); 
                return true;
            }
            catch (MySqlException ex)
            {
                //Handle Error
               Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Execute query (SELECT)
        public DataTable ExecuteQuery(string query)
        {
            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                CloseConnection();
                return dataTable;
            }
            else
            {
                return null;
            }
        }
        //Execute command (INSERT, UPDATE, DELETE)
        public bool ExecuteCommand(string query)
        {
            if (OpenConnection()) 
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
