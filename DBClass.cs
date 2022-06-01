using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    
    public List<string>[] SelectPatient()
    {
        string query = $"SELECT * FROM patients WHERE id_patients != 0";
        List<string>[] list = new List<string>[6];
        for (int i = 0; i < 6; i++)
        {
            list[i] = new List<string>();
        }

        if (this.OpenConnection() == true)
        {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                for (int i = 0; i < 6; i++)
                {
                    string value = dataReader.GetString(i);
                    list[i].Add(value);
                }
            }

            if (list[0].Count != 0)
            {
                List<string>[] result = new List<string>[list[0].Count];

                for (int i = 0; i < list[0].Count; i++)
                {
                    result[i] = new List<string>();
                    for (int k = 0; k < 6; k++)
                    {
                        result[i].Add(list[k][i]);
                    }
                }
                dataReader.Close();
                this.CloseConnection();

                return result;
            }
            return null;
        }
        else
        {
            return list;
        }
    }
    public void UpdatePatient(Patient ptn)
    {
        DateTime parsedDate;
        DateTime.TryParseExact(ptn.born, "d", null, DateTimeStyles.None, out parsedDate);

        string sqlExpression = @"UPDATE patients SET 
            patient_phone=@phone, patient_born=@born, patient_first_name=@name,
            patient_patronymic=@patronymic, patient_last_name=@last_name 
            where id_patients=@id";

        //Open connection
        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(sqlExpression, connection);

            // создаем параметр
            MySqlParameter phoneParam = new MySqlParameter("@phone", ptn.phone);
            // добавляем параметр к команде
            cmd.Parameters.Add(phoneParam);

            MySqlParameter bornParam = new MySqlParameter("@born", parsedDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.Add(bornParam);

            MySqlParameter nameParam = new MySqlParameter("@name", ptn.first_name);
            cmd.Parameters.Add(nameParam);

            MySqlParameter patronymicParam = new MySqlParameter("@patronymic", ptn.patronymic);
            cmd.Parameters.Add(patronymicParam);

            MySqlParameter lastParam = new MySqlParameter("@last_name", ptn.last_name);
            cmd.Parameters.Add(lastParam);

            MySqlParameter idParam = new MySqlParameter("@id", ptn.ID);
            cmd.Parameters.Add(idParam);

            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }
    public void DeletePatient(Patient ptn)
    {
        string query = "DELETE FROM patients WHERE id_patients=@id";

        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlParameter idParam = new MySqlParameter("@id", ptn.ID);
            cmd.Parameters.Add(idParam);


            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1451:
                        MessageBox.Show("Невозможно удалить запись, имеется зависимость в других элементах базы данных");
                        break;
                }
            }

            this.CloseConnection();
        }
    }
    public void CreatePatient(Patient ptn)
    {
        DateTime parsedDate;
        DateTime.TryParseExact(ptn.born, "d", null, DateTimeStyles.None, out parsedDate);

        string sqlExpression = @"INSERT INTO patients 
            (patient_phone, patient_born, patient_first_name,
            patient_patronymic, patient_last_name)
            VALUES(@phone,@born,@name,@patronymic,@last_name)";

        //Open connection
        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(sqlExpression, connection);

            MySqlParameter phoneParam = new MySqlParameter("@phone", ptn.phone);
            cmd.Parameters.Add(phoneParam);

            MySqlParameter bornParam = new MySqlParameter("@born", parsedDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.Add(bornParam);

            MySqlParameter nameParam = new MySqlParameter("@name", ptn.first_name);
            cmd.Parameters.Add(nameParam);

            MySqlParameter patronymicParam = new MySqlParameter("@patronymic", ptn.patronymic);
            cmd.Parameters.Add(patronymicParam);

            MySqlParameter lastParam = new MySqlParameter("@last_name", ptn.last_name);
            cmd.Parameters.Add(lastParam);

            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }


    public List<string>[] SelectTelemetry()
    {
        string query = @"select t.telemetry_time, p.id_patients, t.telemetry_temp, t.telemetry_bar, t.telemetry_charge,
            concat_ws(' ',p.patient_first_name,p.patient_patronymic,p.patient_last_name)
            from patients as p
            join telemetry as t
            on p.id_patients = t.id_patients;";

        //Create a list to store the result
        List<string>[] list = new List<string>[6];
        for (int i = 0; i < 6; i++)
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
                for (int i = 0; i < 6; i++)
                {
                    string value = dataReader.GetString(i);
                    list[i].Add(value);
                }
            }
            if (list[0].Count != 0)
            {
                List<string>[] result = new List<string>[list[0].Count];

                for (int i = 0; i < list[0].Count; i++)
                {
                    result[i] = new List<string>();
                    for (int k = 0; k < 6; k++)
                    {
                        result[i].Add(list[k][i]);
                    }

                }
                dataReader.Close();
                this.CloseConnection();
                return result;
            }
            return null;
        }
        else
        {
            return list;
        }
    }
    public void UpdateTelemetry(Telemetry tel)
    {
        string query = @"UPDATE telemetry SET 
            telemetry_time=@time, id_patients=@pat,
            telemetry_temp=@temp, telemetry_bar=@bar, telemetry_charge=@charge
            where telemetry_time=@time_old and id_patients=@pat_old";

        DateTime parsedDate;
        DateTime.TryParseExact(tel.time, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out parsedDate);
        DateTime parsedDate2;
        DateTime.TryParseExact(tel.time_old, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out parsedDate2);

        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlParameter timeParam = new MySqlParameter("@time", parsedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.Add(timeParam);
            MySqlParameter patientParam = new MySqlParameter("@pat", tel.id_patient);
            cmd.Parameters.Add(patientParam);
            MySqlParameter temptParam = new MySqlParameter("@temp", tel.temp);
            cmd.Parameters.Add(temptParam);
            MySqlParameter barParam = new MySqlParameter("@bar", tel.bar);
            cmd.Parameters.Add(barParam);
            MySqlParameter chargeParam = new MySqlParameter("@charge", tel.charge);
            cmd.Parameters.Add(chargeParam);
            MySqlParameter timeOldParam = new MySqlParameter("@time_old", parsedDate2.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.Add(timeOldParam);
            MySqlParameter patientOldParam = new MySqlParameter("@pat_old", tel.id_patient_old);
            cmd.Parameters.Add(patientOldParam);

            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }

    }
    public void DeleteTelemetry(Telemetry tel)
    {
        string query = "DELETE FROM telemetry WHERE telemetry_time=@time and id_patients=@pat";
        DateTime parsedDate;
        DateTime.TryParseExact(tel.time, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out parsedDate);


        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlParameter timeParam = new MySqlParameter("@time", parsedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.Add(timeParam);
            MySqlParameter patientParam = new MySqlParameter("@pat", tel.id_patient);
            cmd.Parameters.Add(patientParam);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1451:
                        MessageBox.Show("Невозможно удалить запись, имеется зависимость в других элементах базы данных");
                        break;
                }
            }

            this.CloseConnection();
        }
    }
    public void CreateTelemetry(Telemetry tel)
    {
        string query = @"INSERT INTO telemetry 
            (telemetry_time, id_patients, 
            telemetry_temp,telemetry_bar,telemetry_charge)
            VALUES(@time,@pat,@temp,@bar,@charge)";

        DateTime parsedDate;
        DateTime.TryParseExact(tel.time, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out parsedDate);
        DateTime parsedDate2;
        DateTime.TryParseExact(tel.time_old, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out parsedDate2);

        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlParameter timeParam = new MySqlParameter("@time", parsedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.Add(timeParam);
            MySqlParameter patientParam = new MySqlParameter("@pat", tel.id_patient);
            cmd.Parameters.Add(patientParam);
            MySqlParameter temptParam = new MySqlParameter("@temp", tel.temp);
            cmd.Parameters.Add(temptParam);
            MySqlParameter barParam = new MySqlParameter("@bar", tel.bar);
            cmd.Parameters.Add(barParam);
            MySqlParameter chargeParam = new MySqlParameter("@charge", tel.charge);
            cmd.Parameters.Add(chargeParam);

            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }
    public void GetPrimaryKey(out DateTime time, out int id_patient)
    {
        time = DateTime.Now;
        id_patient = 0;
        string? time_string = null;

        string query = @"select telemetry_time, id_patients
                from telemetry
                order by telemetry_time desc
                limit 1;";
        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();


            while (dataReader.Read())
            {
                time_string = dataReader.GetString(0);
                id_patient = dataReader.GetInt32(1);
            }
            dataReader.Close();
            this.CloseConnection();

            DateTime.TryParseExact(time_string, "dd.MM.yyyy H:mm:ss", null, DateTimeStyles.None, out time);
        }

    }
    
    //Рассчитать на время до разряда устройства
    public double GetPreparationTime (int id)
    {
        
        string query = @"
            select telemetry_charge * 0.05 as `time` from telemetry
            where id_patients = @id
            order by telemetry_time desc
            limit 1;";

        double time = 0;

        if (this.OpenConnection() == true)
        {

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlParameter idParam = new MySqlParameter("@id", id);
            cmd.Parameters.Add(idParam);

            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                time = dataReader.GetDouble(0);
            }
            dataReader.Close();
            this.CloseConnection();
            
        }
        return time;
    }

    //топ 3 пациента по количеству принимаемых препаратов(фио кол-во) сортировку по кол-во
    public void GetTopThree(out int[] count, out string[] FIO)
    {
        count = new int[3] {0, 0, 0 };
        FIO = new string[3] {"", "", "" };

        string query = @"
            select concat_ws(' ', p.patient_last_name,p.patient_first_name,p.patient_patronymic) as FIO, 
            sum(pils_count) as `count`
            from plan_receptions as pl
            join patients as p on p.id_patients = pl.id_patient
            group by id_patients
            order by `count` desc, FIO asc
            limit 3;";

        if (this.OpenConnection() == true)
        {
            int i = 0;
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                FIO[i] = dataReader.GetString(0);
                count[i] = dataReader.GetInt32(1);
                i++;
            }
            dataReader.Close();
            this.CloseConnection();
        }

    }
}
