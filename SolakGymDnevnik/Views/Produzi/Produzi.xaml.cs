using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SolakGymDnevnik.Views.Produzi
{
    /// <summary>
    /// Interaction logic for Produzi.xaml
    /// </summary>
    public partial class Produzi : Window
    {
        private Member selectedMember { get; set; }
        private SolakGymDnevnikDataClassesDataContext dataContext;

        public Produzi()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager
                .ConnectionStrings["SolakGymDnevnik.Properties.Settings.SolakGymDnevnikDbConnectionString"].ConnectionString;
            dataContext = new SolakGymDnevnikDataClassesDataContext(connectionString);
        }

        public void Clan(string name, int id)
        {
            selectedMember = dataContext.Members.FirstOrDefault(m => m.Id.Equals(id));
            tbMemberName.Text = name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = DateTime.Today;

            if (DatePicker.SelectedDate.HasValue)
                selectedDate = DatePicker.SelectedDate.Value;

            if (CheckInput(selectedDate))
            {
                selectedMember.ExtendtMembershipByDate(selectedDate);

                dataContext.SubmitChanges();

                var listaWindow = new Lista.Lista();
                listaWindow.Show();
                this.Close();
            }

        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            var listaWindow = new Lista.Lista();
            listaWindow.Show();
            this.Close();
        }

        public bool CheckInput(DateTime date)
        {
            var inputCorrect = true;
            var matchDate = (date - DateTime.Today).Days;

            if (matchDate < 0)
                inputCorrect = false;
                tbMonthIncorrect.Text = "Datum nevažeći!";
                tbMonthIncorrect.Visibility = Visibility.Visible;

            return inputCorrect;
        }

        private void DatePicker_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMonthIncorrect.Visibility = Visibility.Collapsed;
        }
    }
}
