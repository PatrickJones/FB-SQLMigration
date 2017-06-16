using Newtonsoft.Json;
using NuLibrary.Migration.AppEnums;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.Mappings.TableMappings;
using NuLibrary.Migration.SQLDatabase;
using NuLibrary.Migration.SQLDatabase.EF;
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
        NumedicsGlobalHelpers nHelpers = new NumedicsGlobalHelpers();
        ICollection<TableRowCount> sqlRowCount = new List<TableRowCount>();
        String initRowCount = String.Empty;

        BackgroundWorker bWorker = new BackgroundWorker();
        BackgroundWorker bTrans = new BackgroundWorker();

        MappingExecutionManager mm;

        int selectedSiteId = 0;
        List<string> tableNames = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            SetCombo();
            SetSqlDataGrid();

            btnExecute.IsEnabled = false;
            lblStatusBar.Content = "Ready";

            spCompletedMappngs.Visibility = Visibility.Hidden;
            spFailedMappings.Visibility = Visibility.Hidden;
            spMigrationResults.Visibility = Visibility.Hidden;
            stkLoading.Visibility = Visibility.Hidden;

            bWorker.DoWork += BWorker_DoWork;
            bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
            bTrans.DoWork += BTrans_DoWork;
            bTrans.RunWorkerCompleted += BTrans_RunWorkerCompleted;
        }

        private void Reset()
        {
            //SetCombo();
            SetSqlDataGrid();

            listBox.ItemsSource = null;
            dgFailures.ItemsSource = null;
            lblCompCnt.Content = String.Empty;
            lblFailCnt.Content = String.Empty;

            cbxSiteIds.IsEnabled = true;
            btnLoad.IsEnabled = true;
            btnExecute.IsEnabled = false;
            lblStatusBar.Content = "Ready";
        }

        private void SetSqlDataGrid()
        {
            sqlRowCount = nHelpers.GetTableRowCount();
            initRowCount = JsonConvert.SerializeObject(sqlRowCount);
            //dgSqlTables.ItemsSource = sqlRowCount.Select(s => new { TableName = s.TableName, Rows = s.RowCnt });
        }

        private void SetCombo()
        {
            cbxSiteIds.Background = Brushes.Brown;
            cbxSiteIds.Foreground = Brushes.White;
            cbxSiteIds.BorderBrush = Brushes.Brown;
            cbxSiteIds.Resources.Add(SystemColors.WindowBrushKey, Brushes.DarkGoldenrod);

            cbxHistory.Background = Brushes.Brown;
            cbxHistory.Foreground = Brushes.White;
            cbxHistory.BorderBrush = Brushes.Brown;
            cbxHistory.Resources.Add(SystemColors.WindowBrushKey, Brushes.DarkGoldenrod);

            foreach (var r in MigrationVariables.GetRangeDates())
            {
                cbxHistory.Items.Add(r);
            }
            

            foreach (var item in aHelpers.GetAllFirebirdConnections().OrderBy(o => o.SiteId))
            {
                cbxSiteIds.Items.Add(item.SiteId);
            }

            cbxSiteIds.SelectedIndex = 0;
            cbxHistory.SelectedIndex = 3;
            lblSqlConnStr.Content = TransactionManager.DatabaseContext.Database.Connection.ConnectionString;

            UpdateMigrationVars();
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
            DispatchLabel("Creating statistics.");

            var rList = new List<dynamic>();
            var initRC = JsonConvert.DeserializeObject<List<TableRowCount>>(initRowCount);
            NumedicsGlobalHelpers ngh = new NumedicsGlobalHelpers();

            SqlPurge sp = new SqlPurge();
            sp.Purge();

            DispatchLabel("Logging Transaction.");

            MigrationHistoryHelpers mig = new MigrationHistoryHelpers();
            mig.LogMigration();
            TransactionManager.ExecuteTransaction();

            foreach (var table in ngh.GetTableRowCount())
            {
                var preRowCnt = initRC.Where(f => f.TableName == table.TableName).Select(s => s.RowCnt).FirstOrDefault();
                var ms = MappingStatistics.SqlTableStatistics.FirstOrDefault(w => w.Tablename == table.TableName);
                var diff = (ms == null) ? 0 : ms.Difference;
                var n = new
                {
                    TableName = table.TableName,
                    PreMigrationRows = preRowCnt,
                    PostMigratioRows = table.RowCnt,
                    RowsAdded = table.RowCnt - preRowCnt,
                    MappedRowsCommitted = (ms == null) ? 0 : ms.PreSaveCount,
                    MappedRowsSaved = (ms == null) ? 0 : ms.PostSaveCount,
                    MappedRowsDifference = diff,
                    Result = ((table.RowCnt - preRowCnt) == 0) ? "NO CHANGE" : (diff == 0) ? "SUCCESS" : "FAIL"
                };

                rList.Add(n);
            }

            dgMigResults.ItemsSource = rList.OrderBy(o => o.TableName); //MappingStatistics.SqlTableStatistics.OrderBy(o => o.Tablename);

            ((DataGridTextColumn)dgMigResults.Columns[0]).Binding = new Binding("TableName");
            ((DataGridTextColumn)dgMigResults.Columns[1]).Binding = new Binding("PreMigrationRows");
            ((DataGridTextColumn)dgMigResults.Columns[2]).Binding = new Binding("PostMigratioRows");
            ((DataGridTextColumn)dgMigResults.Columns[3]).Binding = new Binding("RowsAdded");
            ((DataGridTextColumn)dgMigResults.Columns[4]).Binding = new Binding("MappedRowsCommitted");
            ((DataGridTextColumn)dgMigResults.Columns[5]).Binding = new Binding("MappedRowsSaved");
            ((DataGridTextColumn)dgMigResults.Columns[6]).Binding = new Binding("MappedRowsDifference");
            ((DataGridTextColumn)dgMigResults.Columns[7]).Binding = new Binding("Result");

            var successCnt = rList.Count(w => w.Result == "SUCCESS");
            var failCnt = rList.Count(w => w.Result == "FAIL");
            var ncCnt = rList.Count(w => w.Result == "NO CHANGE");

            stkLoading.Visibility = Visibility.Hidden;

            lblTblsUpCnt.Content = rList.Count.ToString();
            lblFailUpCnt.Content = failCnt.ToString();
            lblNoChgUpCnt.Content = ncCnt.ToString();
            lblSuccUpCnt.Content = successCnt.ToString();

            lblLogLoc.Content = MigrationVariables.LogFileLocation;

            spMigrationResults.Visibility = Visibility.Visible;
            DispatchLabel("Transaction Complete.");
        }

        private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox.ItemsSource = MappingStatistics.MappingStats
                .OrderBy(o => o.FBTableName)
                .ThenBy(t => t.SQLMappedTable)
                .Where(w => !string.Equals(w.FBTableName, "none", StringComparison.CurrentCultureIgnoreCase)).ToList();
                //.Select(s => new { FbTable = s.FBTableName, FbRows = s.FBRecordCount, SqlTable = s.SQLMappedTable, Completed = s.CompletedMappingsCount, Failed = s.FailedMappingsCount });

            ((DataGridTextColumn)listBox.Columns[0]).Binding = new Binding("FBTableName");
            ((DataGridTextColumn)listBox.Columns[1]).Binding = new Binding("FBRecordCount");
            ((DataGridTextColumn)listBox.Columns[2]).Binding = new Binding("SQLMappedTable");
            ((DataGridTextColumn)listBox.Columns[3]).Binding = new Binding("CompletedMappingsCount");
            ((DataGridTextColumn)listBox.Columns[4]).Binding = new Binding("FailedMappingsCount");

            dgFailures.ItemsSource = MappingStatistics.FailedMappingCollection
                .OrderBy(o => o.SqlTablename)
                .ThenBy(c => c.FailedReason)
                .ThenBy(t => t.ObjectType)
                .Where(w => !string.Equals(w.SqlTablename, "none", StringComparison.CurrentCultureIgnoreCase))
                .Select(s => new {FbTable = s.FBTableName, FbKey = s.FBPrimaryKey, MappingTable = s.SqlTablename, Reason = s.FailedReason, RecordType = s.ObjectType.ToString().Split('.').Last() });

            ((DataGridTextColumn)dgFailures.Columns[0]).Binding = new Binding("FbTable");
            ((DataGridTextColumn)dgFailures.Columns[1]).Binding = new Binding("FbKey");
            ((DataGridTextColumn)dgFailures.Columns[2]).Binding = new Binding("MappingTable");
            ((DataGridTextColumn)dgFailures.Columns[3]).Binding = new Binding("Reason");
            ((DataGridTextColumn)dgFailures.Columns[4]).Binding = new Binding("RecordType");


            lblCompCnt.Content = MappingStatistics.MappingStats.Sum(s => s.CompletedMappingsCount);
            lblFailCnt.Content = MappingStatistics.FailedMappingCollection.Count;

            btnExecute.IsEnabled = true;
            btnNewMigration.IsEnabled = true;
            stkLoading.Visibility = Visibility.Hidden;
            spCompletedMappngs.Visibility = Visibility.Visible;
            spFailedMappings.Visibility = Visibility.Visible;
        }

        private void BTrans_DoWork(object sender, DoWorkEventArgs e)
        {
            mm.UpdateContext();
            DispatchLabel("Beginning transaction...");
            
        }

        private void BWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (tableNames.Count == 0)
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

            DispatchLabel("Mapping tables...");

            mm = new MappingExecutionManager();
            mm.BeginExecution();

            while (!mm.MappingsCompleted)
            {
                
            }


            DispatchLabel("Mapping Complete.");
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            stkLoading.Visibility = Visibility.Visible;
            lblStatusBar.Content = "Loading Firebird table schemas...";

            MigrationVariables.CurrentSiteId = selectedSiteId;
            tableNames = MigrationVariables.FirebirdTableNames.ToList();

            //*TESTING
            tableNames = GetFilteredTableNames();
            MigrationVariables.FirebirdTableNames.Clear();
            MigrationVariables.FirebirdTableNames = tableNames;
            //*TESTING

            bWorker.RunWorkerAsync();

            btnLoad.IsEnabled = false;
            cbxSiteIds.IsEnabled = false;
            btnNewMigration.IsEnabled = false;
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
            stkLoading.Visibility = Visibility.Visible;
            btnNewMigration.Visibility = Visibility.Hidden;
            lblStatusBar.Content = "Executing transaction...";

            //mm.UpdateContext();

            bTrans.RunWorkerAsync();

            btnExecute.IsEnabled = false;
            lblStatusBar.Focus();
        }

        private void cbxSiteIds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int id;
                bool parseSiteId = Int32.TryParse(cbxSiteIds.SelectedItem.ToString(), out id);

                if (parseSiteId)
                {
                    selectedSiteId = id;
                    UpdateLabel($"Current SiteId: {id}");
                    lblFbConnStr.Content = aHelpers.GetAllFirebirdConnections().Where(s => s.SiteId == selectedSiteId).Select(s => s.DatabaseLocation).FirstOrDefault();

                    MigrationVariables.CurrentSiteId = id;
                    UpdateMigrationVars();
                    return;
                }

                UpdateLabel($"Unable to parse Site Id: {cbxSiteIds.SelectedItem.ToString()} into type Int32.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void UpdateMigrationVars()
        {
            lblInstitution.Content = MigrationVariables.Institution;
            lblInitMigDate.Content = MigrationVariables.InitialMigration;
            lblLastMigDate.Content = MigrationVariables.LastMigration;
        }

        private void btnNewMigration_Click(object sender, RoutedEventArgs e)
        {
            spCompletedMappngs.Visibility = Visibility.Hidden;
            spFailedMappings.Visibility = Visibility.Hidden;
            spMigrationResults.Visibility = Visibility.Hidden;

            MappingStatistics.ClearAll();
            MemoryMappings.ClearAll();
            TableAgentCollection.TableAgents.Clear();

            Reset();
        }

        private void cbxHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var si = cbxHistory.SelectedItem.ToString();

            switch (si)
            {
                default:
                    break;
            }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
