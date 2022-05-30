using System;
using System.ComponentModel;

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
}

public class Diagnosis
{
	[DisplayName("Код МКБ-10")]
	public string id { get; set; }
	[DisplayName("Название")]
	public string name { get; set; }
}

public class Telemetry
{
	[DisplayName("Время")]
	public string time { get; set; }
	public int id_patient { get; set; }
	[DisplayName("Температура")]
	public int temp { get; set; }
	[DisplayName("Давление")]
	public int bar { get; set; }
	[DisplayName("Влажность")]
	public int charge { get; set; }
}


