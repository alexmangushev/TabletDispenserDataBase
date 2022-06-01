using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TabletDispenserAdminPanel
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        List<Patient> patient_table;
        public int res;
        public DialogWindow(DBConnect DataBase)
        {
            InitializeComponent();
            patient_table = Patient.GetPatientFromDB(DataBase);
            foreach (Patient pat in patient_table)
            {
                List.Items.Add(pat.first_name+" "+pat.patronymic+" "+pat.last_name);
            }
        }

        private void Button(object sender, RoutedEventArgs e)
        {
            if (List.SelectedIndex != -1)
            {
                res = List.SelectedIndex;
                this.Close();
            }
        }
    }
}
