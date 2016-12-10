using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.TableMappings;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
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
        AspnetDbHelpers aHelpers = new AspnetDbHelpers();

        BackgroundWorker bWorker = new BackgroundWorker();
        BackgroundWorker bTrans = new BackgroundWorker();
        int selectedSiteId = 0;
        List<string> patsTable = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            SetCombo();

            button1.IsEnabled = false;
            label1.Content = "Ready";

            bWorker.DoWork += BWorker_DoWork;
            bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            bTrans.DoWork += BTrans_DoWork;
            bTrans.RunWorkerCompleted += BTrans_RunWorkerCompleted;
        }

        private void SetCombo()
        {
            foreach (var item in aHelpers.GetAllFirebirdConnections().OrderBy(o => o.SiteId))
            {
                comboBox.Items.Add(item.SiteId);
            }

            comboBox.SelectedIndex = 0;
        }

        delegate void UpdateLabelDelegate(string text);
        private void UpdateLabel(string text)
        {
            label1.Content = text;
        }

        private void DispatchLabel(string text)
        {
            UpdateLabelDelegate uLabelDel = new UpdateLabelDelegate(UpdateLabel);
            label1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, uLabelDel, text);
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
            DispatchLabel("Beginning transaction...");
            TransactionManager.ExecuteTransaction();
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DispatchLabel("Populating table...");
            TableAgentCollection.Populate(patsTable);

            DispatchLabel("Mapping table...");
            var pMap = new PatientsMapping();
            pMap.CreatePatientMapping();

            DispatchLabel("Mapping Complete.");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Loading PATIENTS table...";

            MigrationVariables.CurrentSiteId = selectedSiteId;
            patsTable = MigrationVariables.FirebirdTableNames.Where(a => a == "PATIENTS").ToList();

            bWorker.RunWorkerAsync();

            button.IsEnabled = false;
            label1.Content = "PATIENTS table loaded.";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Executing transaction...";
            bTrans.RunWorkerAsync();

            button1.IsEnabled = false;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id;
            bool parseSiteId = Int32.TryParse(comboBox.SelectedItem.ToString(), out id);

            if (parseSiteId)
            {
                selectedSiteId = id;
                UpdateLabel($"Current SiteId: {id}");
                return;
            }

            UpdateLabel($"Unable to parse Site Id: {comboBox.SelectedItem.ToString()} into type Int32.");
        }
    }
}
