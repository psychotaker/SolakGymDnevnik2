using System;
using System.Collections.Generic;
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

namespace SolakGymDnevnik.Views.Prijava
{
    /// <summary>
    /// Interaction logic for Prijava.xaml
    /// </summary>
    public partial class Prijava : Window
    {
        public Prijava()
        {
            InitializeComponent();
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new Glavni.Glavni();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbUserName.Text) && !String.IsNullOrWhiteSpace(pbPassword.Password))
            {
                if (CheckInput(tbUserName.Text, pbPassword.Password) && Admin.IsAdmin(tbUserName.Text, pbPassword.Password))
                {
                    MessageBox.Show("Prijavljeni ste kao administarator", "Prijava uspješna", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    var glavni = new Glavni.Glavni();
                    glavni.BtnPrijava.Visibility = Visibility.Collapsed;
                    glavni.BtnOdjava.Visibility = Visibility.Visible;
                    glavni.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Molimo vas da unesete podatke u odgovarajuća polja", "Unesi podatke",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public bool CheckInput(string Name, string Password)
        {
            var admin = new Admin();
            var inputCorrect = false;
            var adminName = admin.GetAdminName().ToString();
            var adminPw = admin.GetAdminPassword().ToString();
            Match matchPassword = Regex.Match(Password, @"[A-Z]");
            Match matchName = Regex.Match(Name, @"[A-Z]");
            if (!Name.Equals(adminName) && !Password.Equals(adminPw))
            {
                tbUserNameIncorrect.Visibility = Visibility.Visible;
                pbPassword.Margin = new Thickness(0, 120, 0, 0);
                tbPasswordIncorrect.Visibility = Visibility.Visible;
            }
            if (!Name.Equals(adminName))
            {
                tbUserNameIncorrect.Visibility = Visibility.Visible;
                pbPassword.Margin = new Thickness(0, 120, 0, 0);
            }
            else if (Password.Length < 8 || !Password.Equals(adminPw))
            {
                tbPasswordIncorrect.Visibility = Visibility.Visible;
            }
            else
            {
                inputCorrect = true;
            }

            return inputCorrect;
        }

        private void PbPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbPasswordIncorrect.Visibility = Visibility.Collapsed;
        }

        private void TbUserName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbUserNameIncorrect.Visibility = Visibility.Collapsed;
            pbPassword.Margin = new Thickness(0, 70, 0, 0);
        }
    }
}
