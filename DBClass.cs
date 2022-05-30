using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

/// <summary>
/// Summary description for Class1
/// </summary>
public class DBConnect
{
    private MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;

    //Constructor
    public DBConnect()
    {
        Initialize();
    }

    //Initialize values
    private void Initialize()
    {
        server = "localhost";
        database = "tablet_dispenser";
        uid = "root";
        password = "!Mango_567";
        string connectionString;
        connectionString = "SERVER=" + server + ";" + "DATABASE=" +
        database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        connection = new MySqlConnection(connectionString);
    }

    //open connection to database
    public bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number)
            {
                case 0:
                    MessageBox.Show("Cannot connect to server.  Contact administrator");
                    break;

                case 1045:
                    MessageBox.Show("Invalid username/password, please try again");
                    break;
            }
            return false;
        }
    }

    //Close connection
    public bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    //Insert statement
    public void Insert(string table, string columns, string values)
    {
        string query = $"INSERT INTO {table} ({columns}) VALUES({values})";

        //open connection
        if (this.OpenConnection() == true)
        {
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //Execute command
            cmd.ExecuteNonQuery();

            //close connection
            this.CloseConnection();
        }
    }

    //Update statement
    public void Update(string table, string values, string where)
    {
        //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
        string query = $"UPDATE {table} SET {values} WHERE {where}";

        //Open connection
        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }

    //Delete statement
    public void Delete(string table, string where)
    {
        //string query = "DELETE FROM tableinfo WHERE name='John Smith'";
        string query = $"DELETE FROM {table} WHERE {where}";

        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }

    //Select statement
    public List<string>[] Select(string table, string columns, int count_columns, string where)
    {
        string query = $"SELECT {columns} FROM {table} WHERE {where}";

        //Create a list to store the result
        List< string >[] list = new List< string >[count_columns];
        for (int i = 0; i < count_columns; i++)
        {
            list[i] = new List<string>();
        }

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                for (int i = 0; i < count_columns; i++)
                {
                    string value = dataReader.GetString(i);
                    list[i].Add(value);
                }
                //list[0].Add(dataReader["id"] + "");
                //list[1].Add(dataReader["name"] + "");
                //list[2].Add(dataReader["age"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            return list;
        }
        else
        {
            return list;
        }
    }

    //Count statement
    /*public int Count()
    {
    }*/

    //Backup
    public void Backup()
    {
    }

    //Restore
    public void Restore()
    {
    }
}
