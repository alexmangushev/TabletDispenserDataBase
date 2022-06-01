using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Patient
{
    public int ID;
    [DisplayName("Телефон")]
    public string phone { get; set; }
    [DisplayName("Дата рождения")]
    public string born { get; set; }
    [DisplayName("Имя")]
    public string first_name { get; set; }
    [DisplayName("Отчество")]
    public string patronymic { get; set; }
    [DisplayName("Фамилия")]
    public string last_name { get; set; }

    public static List<Patient> GetPatientFromDB(DBConnect DataBase)
    {
        List<string>[] data = DataBase.SelectPatient();
        List<Patient> patient_table = new List<Patient>();
        for (int i = 0; i < data.Length; i++)
        {
            string[] subs = data[i][2].Split(' ');
            Patient pat = new Patient
            {
                phone = data[i][1],
                born = subs[0],
                first_name = data[i][3],
                patronymic = data[i][4],
                last_name = data[i][5]
            };
            pat.ID = int.Parse(data[i][0]);
            patient_table.Add(pat);
        }

        return patient_table;
    }
    public void UpdatePatientFromDB(DBConnect DataBase)
    {
        DataBase.UpdatePatient(this); 
    }
    public void DeletePatientFromDB(DBConnect DataBase)
    {
        DataBase.DeletePatient(this);
    }
    public void CreatePatientIntoDB(DBConnect DataBase)
    {
        DataBase.CreatePatient(this);
    }


}

public class Telemetry
{

    public int id_patient_old;
    public string time_old;

    public int id_patient;
    [DisplayName("Время")]
    public string time { get; set; }
    [DisplayName("ФИО пациента")]
    public string patient { get; set; }
    [DisplayName("Температура")]
    public int temp { get; set; }
    [DisplayName("Давление")]
    public int bar { get; set; }
    [DisplayName("Заряд")]
    public int charge { get; set; }
    public static List<Telemetry> GetTelemetryFromDB(DBConnect DataBase)
    {
        List<string>[] data_2 = DataBase.SelectTelemetry();
        List<Telemetry> telemetry_table = new List<Telemetry>();
        for (int i = 0; i < data_2.Length; i++)
        {
            Telemetry pat = new Telemetry
            {
                time = data_2[i][0],
                time_old = data_2[i][0],
                id_patient = int.Parse(data_2[i][1]),
                id_patient_old = int.Parse(data_2[i][1]),
                temp = int.Parse(data_2[i][2]),
                bar = int.Parse(data_2[i][3]),
                charge = int.Parse(data_2[i][4]),
                patient = data_2[i][5]
            };
            telemetry_table.Add(pat);
        }
        return telemetry_table;
    }
    public void UpdateTelemetryFromDB(DBConnect DataBase)
    {
        DataBase.UpdateTelemetry(this);
    }
    public void DeleteTelemetryFromDB(DBConnect DataBase)
    {
        DataBase.DeleteTelemetry(this);
    }
    public void CreateTelemetryIntoDB(DBConnect DataBase)
    {
        DataBase.CreateTelemetry(this);
    }

    public static void GetNewPrimaryKey(DBConnect DataBase, out string time, out int id_patient)
    {
        time = "01.01.1900 0:00:00";
        id_patient = 0;

        DateTime dateTime = DateTime.Now;
        DataBase.GetPrimaryKey(out dateTime, out id_patient);
        dateTime = dateTime.AddSeconds(1);
        //time = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        time = dateTime.ToString("dd.MM.yyyy H:mm:ss");

    }

}


