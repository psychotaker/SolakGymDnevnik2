using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

namespace SolakGymDnevnik.Views.Lista
{
    /// <summary>
    /// Interaction logic for Lista.xaml
    /// </summary>
    public partial class Lista
    {
        private SolakGymDnevnikDataClassesDataContext dataContext;
        public List<Member> Members { get; set; }

        public Lista()
        {

            InitializeComponent();

            string connectionString = ConfigurationManager
                .ConnectionStrings["SolakGymDnevnik.Properties.Settings.SolakGymDnevnikDbConnectionString"].ConnectionString;
            dataContext = new SolakGymDnevnikDataClassesDataContext(connectionString);

            ShowMembers();
        }

        public void ShowMembers()
        {
            CalculateExpirationTime();
            CreatingMembersListObject();
        }

        public void CreatingMembersListObject()
        {
            var members = dataContext.Members.ToList();

            var sortedMembersByExpirationTime = from member in members orderby member.ExpirationTime select member;
            var sortedMembersByName = from member in members orderby member.Name select member;

            var validMembers = from member in sortedMembersByExpirationTime where member.ExpirationTime > 0 select member;
            var invalidMembers = from member in sortedMembersByName where member.ExpirationTime <= 0 select member;

            lbClanovi.ItemsSource = validMembers;
            lbIstekliClanovi.ItemsSource = invalidMembers;
        }

        public void CalculateExpirationTime()
        {
            var members = dataContext.Members.ToList();
            foreach (var member in members)
            {
                var memberValidationTime = member.MembershipDuration;
                var memberExpirationTime = (memberValidationTime - DateTime.Today).Days;
                member.ExpirationTime = memberExpirationTime;
            }
        }

        public void BtnObrisi_OnClick(object sender, RoutedEventArgs e)
        {
            CreatingMembersListObject();

            if (lbClanovi.SelectedValue != null)
            {

                var selectedValid = (Member)lbClanovi.SelectedValue;
                dataContext.Members.DeleteOnSubmit(selectedValid);
                dataContext.SubmitChanges();
            }
            else if (lbIstekliClanovi.SelectedValue != null)
            {

                var selectedInvalid = (Member)lbIstekliClanovi.SelectedValue;
                dataContext.Members.DeleteOnSubmit(selectedInvalid);
                dataContext.SubmitChanges();
            }
            else
            {
                MessageBox.Show("Označite člana kojeg želite obrisati", "Član nije označen", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            ShowMembers();
        }

        public void BtnUredi_OnClick(object sender, RoutedEventArgs e)
        {
            var urediWindow = new Uredi.Uredi();
            CreatingMembersListObject();


            if (lbClanovi.SelectedValue != null)
            {

                var selectedValid = (Member)lbClanovi.SelectedValue;
                urediWindow.EditMember(selectedValid.Name,selectedValid.MembershipNumber ,selectedValid.PhoneNumber, selectedValid.Id);
                urediWindow.Show();
                Close();
            }
            else if (lbIstekliClanovi.SelectedValue != null)
            {

                var selectedInvalid = (Member)lbIstekliClanovi.SelectedValue;
                urediWindow.EditMember(selectedInvalid.Name,selectedInvalid.MembershipNumber, selectedInvalid.PhoneNumber, selectedInvalid.Id);
                urediWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Označite člana kojeg želite urediti", "Član nije označen", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public void BtnProduzi_OnClick(object sender, RoutedEventArgs e)
        {
            var produziWindow = new Produzi.Produzi();
            var invalidMembers = from member in dataContext.Members where member.ExpirationTime <= 0 select member;
            lbIstekliClanovi.ItemsSource = invalidMembers;

            if (lbIstekliClanovi.SelectedValue != null)
            {
                var selectedInvalid = (Member)lbIstekliClanovi.SelectedValue;
                produziWindow.Clan(selectedInvalid.Name, selectedInvalid.Id);
                produziWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Označite člana kojem je istekla članarina", "Član nije označen", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            dataContext.SubmitChanges();
            ShowMembers();

        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            var glavni = new Glavni.Glavni();
            glavni.Show();
            this.Close();
        }
    }
}
