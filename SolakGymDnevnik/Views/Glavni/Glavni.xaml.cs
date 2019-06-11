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

namespace SolakGymDnevnik.Views.Glavni
{
    /// <summary>
    /// Interaction logic for Glavni.xaml
    /// </summary>
    public partial class Glavni : Window
    {
        public Glavni()
        {
            InitializeComponent();
            AsAdmin();
        }

        private void BtnLista_OnClick(object sender, RoutedEventArgs e)
        {
            if (Admin.CanManage)
            {
                var lista = new Lista.Lista();
                lista.Show();
                this.Close();
            }
            else
            {
                Admin.NotAdminMessage();
            }
        }

        private void BtnNovi_OnClick(object sender, RoutedEventArgs e)
        {
            if (Admin.CanManage)
            {
                var novi = new Novi.Novi();
                novi.Show();
                this.Close();
            }
            else
            {
                Admin.NotAdminMessage();
            }
        }

        private void BtnPrijava_OnClick(object sender, RoutedEventArgs e)
        {
            var prijava = new Prijava.Prijava();
            prijava.Show();
            this.Close();
        }

        private void BtnOdjava_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = MessageBox.Show("Jeste li sigurni da se želite odjaviti?", "Odjava", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (messageBox == MessageBoxResult.Yes)
            {
                Admin.CanManage = false;
                BtnOdjava.Visibility = Visibility.Collapsed;
                BtnPrijava.Visibility = Visibility.Visible;
            }
        }

        public void AsAdmin()
        {
            if (Admin.CanManage)
            {
                BtnPrijava.Visibility = Visibility.Collapsed;
                BtnOdjava.Visibility = Visibility.Visible;
            }
            else
            {
                BtnOdjava.Visibility = Visibility.Collapsed;
                BtnPrijava.Visibility = Visibility.Visible;
            }
        }
    }
}
