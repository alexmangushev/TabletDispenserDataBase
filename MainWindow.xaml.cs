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

    //********************
    // Разнести по классам запросы для bd
    // делаем запрос в бд конкретную таблицу и получаем list не обобщенный, а значений конкретного класса
    // Сделать запросы параметризированными
    // 2 таблицы с crud, 2 аналит запроса
    // во второй таблице выбирать пациента через listwidget
    //*******************
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
                    List<Patient> patient_table = Patient.GetPatientFromDB(DataBase);
                    DBGrid.ItemsSource = patient_table;
                    break;

                case "Телеметрия":
                    List<Telemetry> telemetry_table = Telemetry.GetTelemetryFromDB(DataBase);
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

                case "Телеметрия":
                    Telemetry telemetry = (Telemetry)DBGrid.SelectedItem;
                    row_for_update.Add(DBGrid.Items.IndexOf(telemetry));
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
