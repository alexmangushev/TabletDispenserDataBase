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
        //List<KeyValuePair<string, int>> id_for_save = new List<KeyValuePair<string, int>> ();


        private void ChangeTableComboBox(object sender, SelectionChangedEventArgs e)
        {
            // = SelectTableComboBox.Text;
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)comboBox.SelectedItem;
            TextBlock textComboBox = (TextBlock)item.Content;
            string table = textComboBox.Text;

            DBGrid.Visibility = Visibility.Visible;
            DBGrid.ItemsSource = null;
            string columns;
            string where;
            int column_count;

            current_table_select = table;
            row_for_update.Clear();

            switch (table)
            {
                
                case "Пациенты":
                    //MessageBox.Show(table);
                    

                    columns = "*";
                    where = "id_patients != 0";
                    column_count = 6;
                    List<string>[] data = DataBase.Select("patients", columns, column_count, where);

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
                    

                    DBGrid.ItemsSource = patient_table;
                    break;

                case "Диагнозы":
                    columns = "*";
                    where = "icd_10_code is not null";
                    column_count = 2;
                    List<string>[] data_1 = DataBase.Select("diagnosis", columns, column_count, where);

                    List<Diagnosis> diagnosis_table = new List<Diagnosis>();
                    for (int i = 0; i < data_1.Length; i++)
                    {
                        Diagnosis pat = new Diagnosis
                        {
                            id = data_1[i][0],
                            name = data_1[i][1],
                            
                        };
                        diagnosis_table.Add(pat);
                    }


                    DBGrid.ItemsSource = diagnosis_table;
                    break;

                case "Телеметрия":
                    columns = "*";
                    where = "telemetry_time is not null";
                    column_count = 5;
                    List<string>[] data_2 = DataBase.Select("telemetry", columns, column_count, where);

                    List<Telemetry> telemetry_table = new List<Telemetry>();
                    for (int i = 0; i < data_2.Length; i++)
                    {
                        Telemetry pat = new Telemetry
                        {
                            time = data_2[i][0],
                            id_patient = int.Parse(data_2[i][1]),
                            temp = int.Parse(data_2[i][2]),
                            bar = int.Parse(data_2[i][3]),
                            charge = int.Parse(data_2[i][4])
                        };
                        telemetry_table.Add(pat);
                    }

                    DBGrid.ItemsSource = telemetry_table;
                    break;
            }

        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            string table_updt;
            string where;
            string values;

            switch (current_table_select)
            {

                case "Пациенты":
                    Patient pat;
                    foreach (int i in row_for_update)
                    {
                        pat = (Patient)DBGrid.Items.GetItemAt(i);
                        DateTime parsedDate;
                        DateTime.TryParseExact(pat.born, "d", null, DateTimeStyles.None, out parsedDate);

                        table_updt = "patients";
                        where = $"id_patients = \"{pat.ID}\"";
                        values = $"patient_phone = \"{pat.phone}\", patient_born = \"{parsedDate.ToString("yyyy-MM-dd")}\", " +
                            $"patient_first_name = \"{pat.first_name}\", patient_patronymic = \"{pat.patronymic}\", " +
                            $"patient_last_name = \"{pat.last_name}\"";
                        DataBase.Update(table_updt,values,where);
                    }
                    break;

                case "Диагнозы":
                    

                    break;

                case "Телеметрия":
                    

                    break;
            }
        }
        /*
        private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null) return;

        }*/

        private void EditCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            switch (current_table_select)
            {

                case "Пациенты":
                    Patient pat = (Patient)DBGrid.SelectedItem;
                    row_for_update.Add(DBGrid.Items.IndexOf(pat));
                    break;

                case "Диагнозы":
                    Diagnosis diagnosis = (Diagnosis)DBGrid.SelectedItem;
                    //id_for_save.Add(diagnosis.id);
                    break;

                case "Телеметрия":
                    Telemetry telemetry = (Telemetry)DBGrid.SelectedItem;
                    //id_for_save.Add(telemetry.time);
                    break;
            }
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
