using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TabletDispenserAdminPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        DBConnect DataBase;
        public MainWindow()
        {
            InitializeComponent();
            DataBase = new DBConnect();
            DataBase.OpenConnection();
            DataBase.CloseConnection();
        }

        public string current_table_select;
        List<int> row_for_update = new List<int>();
        public int id_from_another_window { get; set; }
        List<Patient> patient_table;
        List<Telemetry> telemetry_table;

        private void ChangeTableComboBox(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)comboBox.SelectedItem;
            TextBlock textComboBox = (TextBlock)item.Content;
            string table = textComboBox.Text;

            DBGrid.Visibility = Visibility.Visible;
            DBGrid.ItemsSource = null;

            current_table_select = table;
            row_for_update.Clear();

            switch (table)
            {
                
                case "Пациенты":
                    patient_table = Patient.GetPatientFromDB(DataBase);
                    DBGrid.ItemsSource = patient_table;
                    break;

                case "Телеметрия":
                    telemetry_table = Telemetry.GetTelemetryFromDB(DataBase);
                    DBGrid.ItemsSource = telemetry_table;
                    break;
            }

        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            switch (current_table_select)
            {
                case "Пациенты":
                    Patient pat;
                    foreach (int i in row_for_update)
                    {
                        pat = (Patient)DBGrid.Items.GetItemAt(i);
                        pat.UpdatePatientFromDB(DataBase);
                        patient_table = Patient.GetPatientFromDB(DataBase);
                    }
                    break;

                case "Телеметрия":
                    Telemetry tel;
                    foreach (int i in row_for_update)
                    {
                        tel = (Telemetry)DBGrid.Items.GetItemAt(i);
                        tel.UpdateTelemetryFromDB(DataBase);
                        telemetry_table = Telemetry.GetTelemetryFromDB(DataBase);
                    }
                    break;
            }
        }
       
        private void EditCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            switch (current_table_select)
            {

                case "Пациенты":
                    Patient pat = (Patient)DBGrid.SelectedItem;
                    row_for_update.Add(DBGrid.Items.IndexOf(pat));
                    break;

                case "Телеметрия":
                    Telemetry telemetry = (Telemetry)DBGrid.SelectedItem;
                    row_for_update.Add(DBGrid.Items.IndexOf(telemetry));
                    break;
            }
        }
        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            switch (current_table_select)
            {
                case "Пациенты":
                    Patient pat = (Patient)DBGrid.SelectedItem;
                    pat.DeletePatientFromDB(DataBase);
                    patient_table = Patient.GetPatientFromDB(DataBase);
                    DBGrid.ItemsSource = patient_table;
                    break;

                case "Телеметрия":
                    Telemetry tel = (Telemetry)DBGrid.SelectedItem;
                    tel.DeleteTelemetryFromDB(DataBase);
                     telemetry_table = Telemetry.GetTelemetryFromDB(DataBase);
                    DBGrid.ItemsSource = telemetry_table;
                    break;
            }
        }

        private void CreateButton(object sender, RoutedEventArgs e)
        {
            switch (current_table_select)
            {
                case "Пациенты":
                    Patient pat = new Patient { phone="0-000-000-00", born="01.01.1900", 
                        first_name="Нулл",last_name= "Нулл",patronymic= "Нулл"
                    };
                    pat.CreatePatientIntoDB(DataBase);
                    patient_table = Patient.GetPatientFromDB(DataBase);
                    DBGrid.ItemsSource = patient_table;
                    break;

                case "Телеметрия":
                    string time;
                    int id_patient;
                    Telemetry.GetNewPrimaryKey(DataBase, out time, out id_patient);
                    Telemetry tel = new Telemetry { 
                        id_patient = id_patient, time= time, 
                        temp=-1,bar=0,charge=101 };
                    tel.CreateTelemetryIntoDB(DataBase);
                    telemetry_table = Telemetry.GetTelemetryFromDB(DataBase);
                    DBGrid.ItemsSource = telemetry_table; 
                    break;
            }
        }
        private void DBGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (current_table_select == "Телеметрия")
            {
                Telemetry tel = (Telemetry)DBGrid.SelectedItem;
                int cur_index = DBGrid.Items.IndexOf(tel);
                DialogWindow choose = new DialogWindow(DataBase);
                patient_table = Patient.GetPatientFromDB(DataBase);
                for (int i = 0; i < patient_table.Count; i++)
                {
                    if (patient_table[i].ID == tel.id_patient)
                        choose.res = i;
                }
                choose.ShowDialog();
                telemetry_table[cur_index].id_patient = patient_table[choose.res].ID;
                telemetry_table[cur_index].patient = patient_table[choose.res].first_name + " "
                    + patient_table[choose.res].patronymic + " " + patient_table[choose.res].last_name;
            }
        }

        private void TextBoxPatient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            patient_table = Patient.GetPatientFromDB(DataBase);
            DialogWindow choose = new DialogWindow(DataBase);
            choose.ShowDialog();
            TextBoxPatient.Text = patient_table[choose.res].first_name + " "
                    + patient_table[choose.res].patronymic + " " + patient_table[choose.res].last_name;
            
            TextBoxAns.Text = "Время до разряда устройства = " + Math.Round(DataBase.GetPreparationTime(choose.res), 3).ToString() + " Часов";
        }

        private void ListBoxAns_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string[] FIO;
            int[] count;
            DataBase.GetTopThree(out count, out FIO);
            ListBoxAns.Items.Clear();
            ListBoxAns.Items.Add(FIO[0] + " - " + count[0] + " принимаемых препаратов");
            ListBoxAns.Items.Add(FIO[1] + " - " + count[1] + " принимаемых препаратов");
            ListBoxAns.Items.Add(FIO[2] + " - " + count[2] + " принимаемых препаратов");
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(0);
        }

        // This snippet can be used if you can be sure that every
        // member will be decorated with a [DisplayNameAttribute]
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
            => e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor)?.DisplayName ?? e.Column.Header;

    }
}
