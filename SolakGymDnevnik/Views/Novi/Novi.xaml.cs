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

namespace SolakGymDnevnik.Views.Novi
{
    /// <summary>
    /// Interaction logic for Novi.xaml
    /// </summary>
    public partial class Novi
    {
        private SolakGymDnevnikDataClassesDataContext dataContext;
        public Novi()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager
                .ConnectionStrings["SolakGymDnevnik.Properties.Settings.SolakGymDnevnikDbConnectionString"].ConnectionString;
            dataContext = new SolakGymDnevnikDataClassesDataContext(connectionString);
        }
        public void AddMember()
        {
            var selectedDate = DateTime.Today;

            if (DatePicker.SelectedDate.HasValue)
                selectedDate = DatePicker.SelectedDate.Value;

            try
            {
                if (!String.IsNullOrWhiteSpace(tbMembershipNumber.Text) && !String.IsNullOrWhiteSpace(tbName.Text) && !String.IsNullOrWhiteSpace(tbPhoneNumber.Text))
                {
                    if (CheckInput(tbName.Text, tbMembershipNumber.Text, tbPhoneNumber.Text, selectedDate))
                    {
                        var newMemeber = new Member(tbName.Text, Convert.ToInt32(tbMembershipNumber.Text), tbPhoneNumber.Text, selectedDate);
                        dataContext.Members.InsertOnSubmit(newMemeber);
                        dataContext.SubmitChanges();
                        tbName.Text = null;
                        tbMembershipNumber.Text = null;
                        tbPhoneNumber.Text = null;
                        DatePicker.SelectedDate = null;
                    }
                }
                else
                {
                    MessageBox.Show("Unesite zadana polja", "Pogreška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ime i prezime, broj telefona i mjeseci su obavezni, provjerite da li su sva polja ispravno unešena", "Obavezna polja", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public bool CheckInput(string Name, string MembershipNumber, string PhoneNumber, DateTime Date)
        {
            var inputCorrect = false;
            bool contains = dataContext.Members.AsEnumerable().Any(row => Convert.ToInt32(MembershipNumber) == row.MembershipNumber);
            Match matchPhoneNumber = Regex.Match(PhoneNumber, @"\d");
            Match matchName = Regex.Match(Name, @"[A-Z]");
            Match matchMembershipNumber = Regex.Match(MembershipNumber, @"\d");
            var matchDate = (Date - DateTime.Today).Days;
            var MembershipNumberValue = Convert.ToInt32(MembershipNumber);
            if (!matchName.Success && !matchMembershipNumber.Success && !matchPhoneNumber.Success)
            {
                tbNameIncorrect.Visibility = Visibility.Visible;
                tbMembershipNumberIncorrect.Visibility = Visibility.Visible;
                tbPhoneNumberIncorrect.Visibility = Visibility.Visible;
                tbMonthIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchName.Success)
            {
                tbNameIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchPhoneNumber.Success || PhoneNumber.Length < 9)
            {
                tbPhoneNumberIncorrect.Visibility = Visibility.Visible;
            }
            else if (matchDate < 0)
            {
                tbMonthIncorrect.Text = "Datum nevažeći!";
                tbMonthIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchMembershipNumber.Success || MembershipNumberValue < 1)
            {
                tbMembershipNumberIncorrect.Text = "Unesi članski broj!";
                tbMembershipNumberIncorrect.Visibility = Visibility.Visible;
            }
            else if (contains)
            {
                tbMembershipNumberIncorrect.Text = "Članski broj zauzet!";
                tbMembershipNumberIncorrect.Visibility = Visibility.Visible;
            }
            else
            {
                inputCorrect = true;
            }

            return inputCorrect;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddMember();
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new Glavni.Glavni();
            mainWindow.Show();
            this.Close();
        }

        private void TbPhoneNumber_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbPhoneNumberIncorrect.Visibility = Visibility.Collapsed;
        }

        private void TbName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbNameIncorrect.Visibility = Visibility.Collapsed;
        }

        private void DatePicker_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMonthIncorrect.Visibility = Visibility.Collapsed;
        }

        private void TbMembershipNumber_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMembershipNumberIncorrect.Visibility = Visibility.Collapsed;
        }
    }
}
