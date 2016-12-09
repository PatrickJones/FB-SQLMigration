using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.TableMappings;
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

namespace MigrationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker bWorker = new BackgroundWorker();
        BackgroundWorker bTrans = new BackgroundWorker();
        int selectedSiteid;
        List<string> patsTable = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            button1.IsEnabled = false;
            label1.Content = "Ready";

            bWorker.DoWork += BWorker_DoWork;
            bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            bTrans.DoWork += BTrans_DoWork;
            bTrans.RunWorkerCompleted += BTrans_RunWorkerCompleted;
        }

        private void BTrans_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Content = "Transaction Complete.";
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.IsEnabled = true;
        }

        private void BTrans_DoWork(object sender, DoWorkEventArgs e)
        {
            TransactionManager.ExecuteTransaction();
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            TableAgentCollection.Populate(patsTable);

            var pMap = new PatientsMapping();
            pMap.CreatePatientMapping();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Loading PATIENTS table...";

            MigrationVariables.CurrentSiteId = Int32.Parse(textBox.Text);
            patsTable = MigrationVariables.FirebirdTableNames.Where(a => a == "PATIENTS").ToList();

            bWorker.RunWorkerAsync();
            //MigrationVariables.CurrentSiteId = Int32.Parse(textBox.Text);
            //var patsTable = MigrationVariables.FirebirdTableNames.Where(a => a == "PATIENTS").ToList();

            //TableAgentCollection.Populate(patsTable);

            //var pMap = new PatientsMapping();
            //pMap.CreatePatientMapping();

            button.IsEnabled = false;
            label1.Content = "PATIENTS table loaded.";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Executing transaction...";
            bTrans.RunWorkerAsync();
            //TransactionManager.ExecuteTransaction();
            button1.IsEnabled = false;
        }
    }
}
