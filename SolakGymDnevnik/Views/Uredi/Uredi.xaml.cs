using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SolakGymDnevnik.Views.Uredi
{
    /// <summary>
    /// Interaction logic for Uredi.xaml
    /// </summary>
    public partial class Uredi : Window
    {
        private Member selectedMember { get; set; }
        private SolakGymDnevnikDataClassesDataContext dataContext;
        public Uredi()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager
                .ConnectionStrings["SolakGymDnevnik.Properties.Settings.SolakGymDnevnikDbConnectionString"].ConnectionString;
            dataContext = new SolakGymDnevnikDataClassesDataContext(connectionString);
        }

        public void EditMember(string userName,int membershipNumber, string phoneNumber,int id)
        {
            selectedMember = dataContext.Members.FirstOrDefault(m => m.Id.Equals(id));
            tbName.Text = userName;
            tbMembershipNumber.Text = membershipNumber.ToString();
            tbPhoneNumber.Text = phoneNumber;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput(tbName.Text,tbMembershipNumber.Text, tbPhoneNumber.Text))
            {
                selectedMember.Name = tbName.Text;
                selectedMember.MembershipNumber = Convert.ToInt32(tbMembershipNumber.Text);
                selectedMember.PhoneNumber = tbPhoneNumber.Text;

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

        private void TbMembershipNumber_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMembershipNumberIncorrect.Visibility = Visibility.Collapsed;
        }

        public bool CheckInput(string Name, string MembershipNumber, string PhoneNumber)
        {
            var inputCorrect = false;
            bool contains = dataContext.Members.AsEnumerable().Any(row => Convert.ToInt32(MembershipNumber) == row.MembershipNumber);
            Match matchPhoneNumber = Regex.Match(PhoneNumber, @"\d");
            Match matchName = Regex.Match(Name, @"[A-Z]");
            Match matchMembershipNumber = Regex.Match(MembershipNumber, @"\d");
            var MembershipNumberValue = Convert.ToInt32(MembershipNumber);
            if (!matchName.Success && !matchPhoneNumber.Success && !matchMembershipNumber.Success)
            {
                tbNameIncorrect.Visibility = Visibility.Visible;
                tbPhoneNumberIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchName.Success)
            {
                tbNameIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchPhoneNumber.Success || PhoneNumber.Length < 9)
            {
                tbPhoneNumberIncorrect.Visibility = Visibility.Visible;
            }
            else if (!matchMembershipNumber.Success || MembershipNumberValue < 1)
            {
                tbMembershipNumberIncorrect.Text = "Unesi članski broj!";
                tbMembershipNumberIncorrect.Visibility = Visibility.Visible;
            }
            else if (contains && selectedMember.MembershipNumber != Convert.ToInt32(MembershipNumber))
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
    }
}
