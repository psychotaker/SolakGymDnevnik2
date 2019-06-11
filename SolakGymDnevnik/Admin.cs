using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolakGymDnevnik
{
    class Admin
    {
        private string Name { get; set; }
        private string Password { get; set; }
        private static bool _isAdmin = false;
        public static bool CanManage = _isAdmin;

        public Admin()
        {
            Name = "adminedin";
            Password = "adminsgd";
        }


        //TODO Add admin for application
        public string GetAdminName()
        {
            return Name;
        }

        public string GetAdminPassword()
        {
            return Password;
        }

        public static bool IsAdmin(string username,string password)
        {
            var admin = new Admin();
            if (username == admin.GetAdminName() && password == admin.GetAdminPassword())
            {
                _isAdmin = true;
                CanManage = _isAdmin;
            }

            return _isAdmin;
        }

        public static void NotAdminMessage()
        {
            MessageBox.Show("Da bi ste koristili ovu uslugu morate biti prijavljeni kao administrator", "Niste prijavljeni", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
