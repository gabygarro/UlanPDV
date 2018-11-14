using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace UlanPDV
{
    class Database
    {
        private MySqlConnection connection;
        private MySqlCommand command;
        private MySqlDataReader reader;
        private bool connected;

        private string squery;
        private List<string> parameters;
        private int affected_rows;
        private DataTable Table;

        readonly string connectionString = "Server=127.0.0.1;Database=ulanpdv;Uid=UlanPDV;Pwd=ulan-PDV11;";

        public Database()
        {
            this.connect();
            this.parameters = new List<string>();
        }

        public void connect()
        {
            try
            {
                this.connection = new MySqlConnection(this.connectionString);
                this.connection.Open();
                this.connected = true;
            }
            catch (MySqlException ex)
            {
                DebugQuery(ex.Message.ToString());
            }
        }

        public void CloseConnection()
        {
            this.connected = false;
            this.connection.Close();
            this.connection.Dispose();
        }

        public bool Login(string username, string password)
        {
            string[] exists = this.row("SELECT COUNT(*) FROM user WHERE username = @username AND password = @password",
                new string[] { "username", username, "password", password });
            if (exists[0] == "0")
            {
                return false;
            }
            return true;
        }

        private void Init(string query, string[] bindings = null)
        {
            if (this.connected == false)
            {
                this.connect();
            }
            // Automatically disposes the MySQLcommand instance
            using (this.command = new MySqlCommand(query, this.connection))
            {
                this.bind(bindings);
                // Prevents SQL injection
                if (this.parameters.Count > 0)
                {
                    this.parameters.ForEach(delegate (string parameter)
                    {
                        string[] sparameters = parameter.ToString().Split('\x7F');
                        this.command.Parameters.AddWithValue(sparameters[0], sparameters[1]);
                    });
                }
                this.squery = query.ToLower();

                if (squery.Contains("select"))
                {
                    this.Table = execDatatable();
                }
                if (squery.Contains("delete") || squery.Contains("update") || squery.Contains("insert"))
                {
                    this.affected_rows = execNonquery();
                }

                this.parameters.Clear();
            }
        }

        public DataTable query(string query, string[] bindings = null)
        {
            this.Init(query, bindings);
            return this.Table;
        }

        public int nQuery(string query, string[] bindings = null)
        {
            this.Init(query, bindings);
            return this.affected_rows;
        }

        public string[] row(string query, string[] bindings = null)
        {
            this.Init(query, bindings);
            string[] row = new string[this.Table.Columns.Count];
            if (this.Table.Rows.Count > 0)
                for (int i = 0; i++ < this.Table.Columns.Count; row[i - 1] = this.Table.Rows[0][i - 1].ToString()) ;
            return row;
        }

        public void bind(string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this.bind(fields[i], fields[i + 1]);
                    i += 1;
                }
            }
        }

        public void bind(string field, string value)
        {
            this.parameters.Add("@" + field + "\x7F" + value);
        }

        private DataTable execDatatable()
        {
            DataTable dt = new DataTable();
            try
            {
                this.reader = command.ExecuteReader();
                dt.Load(reader);
            }
            catch (MySqlException my)
            {
                DebugQuery(my.Message.ToString());
            }
            return dt;
        }

        private int execNonquery()
        {
            int affected = 0;
            try
            {
                affected = command.ExecuteNonQuery();
            }
            catch (MySqlException my)
            {
                DebugQuery(my.Message.ToString());
            }

            return affected;
        }

        private void DebugQuery(string error)
        {
            string exception = "Error: " + error + "\n\rQuery:\n\r" + squery;
            MessageBox.Show(exception, "Error de base de datos");
            Console.WriteLine(error + "/n/r");
        }
    }
}
