using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SolakGymDnevnik
{
    [RunInstaller(true)]
    public partial class SetupInstaller : Installer
    {
        SqlConnection masterConnection = new SqlConnection();
        public SetupInstaller() : base()
        {
            InitializeComponent();
            masterConnection.ConnectionString = Properties.Settings.Default.masterConnectionString;
        }
        private string GetSql(string Name)
        {

            try
            {
                // Gets the current assembly.
                Assembly Asm = Assembly.GetExecutingAssembly();

                // Resources are named using a fully qualified name.
                Stream strm = Asm.GetManifestResourceStream(Asm.GetName().Name + "." + Name);

                // Reads the contents of the embedded file.
                StreamReader reader = new StreamReader(strm);
                return reader.ReadToEnd();

            }
            catch (Exception ex)
            {
                MessageBox.Show("In GetSQL: " + ex.Message);
                throw ex;
            }
        }

        private void ExecuteSql(string DatabaseName, string Sql)
        {
            SqlCommand Command = new SqlCommand(Sql, masterConnection);

            // Initialize the connection, open it, and set it to the "master" database
            masterConnection.ConnectionString = Properties.Settings.Default.masterConnectionString;
            Command.Connection.Open();
            Command.Connection.ChangeDatabase(DatabaseName);
            try
            {
                Command.ExecuteNonQuery();
            }
            finally
            {
                // Closing the connection should be done in a Finally block
                Command.Connection.Close();
            }
        }

        protected void AddDBTable(string strDBName)
        {

            try
            {
                if (!CheckDatabaseExists(masterConnection.ConnectionString, strDBName))
                {
                    // Creates the database.
                    ExecuteSql("master", "CREATE DATABASE " + strDBName);

                    // Creates the tables.
                    ExecuteSql(strDBName, GetSql("tables.txt"));

                    // Creates the stored procedure.
                    ExecuteSql(strDBName, GetSql("getmember.txt"));
                }
                else
                {
                    var messageBox = MessageBox.Show("Želite li obrisati već postojeću bazu SolakGymDnevnikDb?", "Obriši i kreiraj novu bazu", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (messageBox == MessageBoxResult.Yes)
                    {
                        ExecuteSql("master", GetSql("dropcreate.txt"));
                    }
                }

            }
            catch (Exception ex)
            {
                // Reports any errors and abort.
                MessageBox.Show("In exception handler: " + ex.Message);
                throw ex;
            }
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            AddDBTable("SolakGymDnevnikDb");

        }

        //[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            var messageBox = MessageBox.Show("Želite li obrisati bazu SolakGymDnevnikDb?", "Obriši bazu", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (messageBox == MessageBoxResult.Yes)
            {
                //var messageBoxBackUp = MessageBox.Show("Želite li napraviti backup baze SolakGymDnevnikDb?", "Backup", MessageBoxButton.YesNo,
                //MessageBoxImage.Question);
                //if (messageBoxBackUp == MessageBoxResult.Yes)
                //{
                //    var backuppath = @"'C:\Users\Teufik\Documents\SolakGymDnevnikDb.bak'";
                //    var backupstring = "BACKUP DATABASE SolakGymDnevnikDb TO DISK = "+backuppath;
                //    ExecuteSql("master", "USE SolakGymDnevnikDb");
                //    ExecuteSql("master", backupstring);
                //}
                ExecuteSql("master", "DROP DATABASE SolakGymDnevnikDb");
            }
        }

        public static bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }
    }
}
