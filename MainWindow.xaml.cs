using System;
using System.Collections.Generic;
using System.ComponentModel;
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



        private void ChangeTableComboBox(object sender, SelectionChangedEventArgs e)
        {
            // = SelectTableComboBox.Text;
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)comboBox.SelectedItem;
            TextBlock textComboBox = (TextBlock)item.Content;
            string table = textComboBox.Text;
            
            switch(table)
            {
                case "Пациенты":
                    MessageBox.Show(table);
                    string columns = "patient_first_name, patient_patronymic, patient_last_name";
                    string where = "id_patients = 5";
                    int column_count = 3;
                    List<string>[] data = DataBase.Select("patients", columns, column_count, where);
                    //DBGrid
                    break;
            }

            /*MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
                "pwd=!Mango_567;database=tablet_dispenser";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }*/
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(0);
        }
    }
}
