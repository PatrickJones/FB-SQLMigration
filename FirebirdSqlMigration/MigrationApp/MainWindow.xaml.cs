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
        List<string> tableNames = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            SetCombo();

            btnExecute.IsEnabled = false;
            lblStatusBar.Content = "Ready";

            bWorker.DoWork += BWorker_DoWork;
            bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            bTrans.DoWork += BTrans_DoWork;
            bTrans.RunWorkerCompleted += BTrans_RunWorkerCompleted;
        }

        private void SetCombo()
        {
            foreach (var item in aHelpers.GetAllFirebirdConnections().OrderBy(o => o.SiteId))
            {
                cbxSiteIds.Items.Add(item.SiteId);
            }

            cbxSiteIds.SelectedIndex = 0;
        }

        delegate void UpdateLabelDelegate(string text);
        private void UpdateLabel(string text)
        {
            lblStatusBar.Content = text;
        }

        private void DispatchLabel(string text)
        {
            UpdateLabelDelegate uLabelDel = new UpdateLabelDelegate(UpdateLabel);
            lblStatusBar.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, uLabelDel, text);
        }

        private void BTrans_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatusBar.Content = "Transaction Complete.";
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnExecute.IsEnabled = true;
        }

        private void BTrans_DoWork(object sender, DoWorkEventArgs e)
        {
            DispatchLabel("Beginning transaction...");
            TransactionManager.ExecuteTransaction();
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (tableNames.Count() == 0)
            {
                DispatchLabel("Populating all tables...");
                TableAgentCollection.Populate();
            }
            else
            {
                TableAgentCollection.Populate(tableNames);
                Array.ForEach(tableNames.ToArray(), a => {
                    DispatchLabel($"Populating table {a}...");
                });
            }    

            DispatchLabel("Mapping table...");
            MappingExecutionManager mm = new MappingExecutionManager();
            mm.BeginExecution();

            DispatchLabel("Mapping Complete.");
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            lblStatusBar.Content = "Loading Firebird table schemas...";

            MigrationVariables.CurrentSiteId = selectedSiteId;
            tableNames = MigrationVariables.FirebirdTableNames.ToList();

            //*TESTING
            tableNames = GetFilteredTableNames();
            //*TESTING

            bWorker.RunWorkerAsync();

            btnLoad.IsEnabled = false;
            lblStatusBar.Content = "Schemas loaded.";
        }

        private List<string> GetFilteredTableNames()
        {
            return new List<string> {
                "DMDATA",
                "INSULETPUMPSETTINGS",
                "INSURANCECOS",
                "INSURANCEPLANS2",
                "METERREADING",
                "METERREADINGHEADER",
                "NDCS",
                "PATIENTPUMP",
                "PATIENTPUMPPROGRAM",
                "PATIENTS",
                "PHONENUMBERS",
                "PUMPTIMESLOTS",
                "TIMESLOT",
                "NULICENSE"
            };
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            lblStatusBar.Content = "Executing transaction...";
            bTrans.RunWorkerAsync();

            btnExecute.IsEnabled = false;
        }

        private void cbxSiteIds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id;
            bool parseSiteId = Int32.TryParse(cbxSiteIds.SelectedItem.ToString(), out id);

            if (parseSiteId)
            {
                selectedSiteId = id;
                UpdateLabel($"Current SiteId: {id}");
                return;
            }

            UpdateLabel($"Unable to parse Site Id: {cbxSiteIds.SelectedItem.ToString()} into type Int32.");
        }

    }
}
