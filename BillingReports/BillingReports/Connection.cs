using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;


namespace BillingReports
{
    class Connection
    {
        public string ConnectionString;
        public string ServerVersion;
        public MySqlConnection GlobalConnection;
        public Connection()
        {

            using var connect = new MySqlConnection(ConnectionString);
            connect.Open();
            GlobalConnection = connect;


            ServerVersion = connect.ServerVersion;
            connect.Close();

        }
    }
}
