﻿using NuLibrary.Migration.AppEnums;
using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.TableMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SqlValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            //var cls = new FBDataAccess(21002);
            //cls.GetTableNames();
            //var cls = new TableAgent(999); //localhost
            //cls.GetTableSchema();
            //cls.GetData();
            //var table = cls.GetDataTable();
            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //    var col = table.Columns[i];
            //    System.Console.WriteLine(String.Format("Column Name: {0} - Column Type: {1}", col.ColumnName, col.DataType.Name));
            //}
            //var tac = new TableAgentCollection(999); Local



            //MigrationVariables.CurrentSiteId = 20001;
            //var t = TableAgentCollection.TableAgents;
            //System.Console.ReadLine();

            //TestMeterReadings();
            //TestTransaction();
            //TestValidation();
            //CreateEnums();
            //TestPatientTransaction();
            //TestInstitutionMapping();
            //TestClinicianMapping();
            //TestDmdataMapping();

            PatientLinks();

            //double ans = (Double)9 / (Double)4;
            //System.Console.WriteLine(ans);
            //System.Console.ReadLine();
        }

        private static void PatientLinks()
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                System.Console.WriteLine(ctx.Users.Where(w => w.Patient == null && w.UserType == 2).Count());
                
                System.Console.ReadLine();
            }
        }

        private static void TestMeterReadings()
        {
            var fb = new FBDataAccess();
            var table = fb.GetData();

            var first = table.Rows[0];
            var split = first["METERSENT"].ToString().Split('\t').ToList();

            int total = (split.FindIndex(a => a == "Total")) + 1;
            int carbs = (split.FindIndex(a => a == "Carbs")) + 1;
            int icRatio = (split.FindIndex(a => a == "IC Ratio")) + 1;
            int targetBg = (split.FindIndex(a => a == "Target BG")) + 1;
            int actualBg = (split.FindIndex(a => a == "BG")) + 1;
            int correctAbove = (split.FindIndex(a => a == "Correction Above")) + 1;
            int correct = (split.FindIndex(a => a == "Correct")) + 1;
            int volume = (split.FindIndex(a => a == "Volume")) + 1;
            int imDuration = (split.FindIndex(a => a == "Immediate Duration")) + 1;
            int exDuration = (split.FindIndex(a => a == "Extended Duration")) + 1;
            int correction = (split.FindIndex(a => a == "Correction")) + 1;
            int correctionIOB = (split.FindIndex(a => a == "Correction Insulin on Board")) + 1;
            int mealIOB = (split.FindIndex(a => a == "Meal Insulin on Board")) + 1;
            int revCorrection = (split.FindIndex(a => a == "Reverse Correction")) + 1;
            int progCorrection = (split.FindIndex(a => a == "Programmed Correction")) + 1;
            int progMeal = (split.FindIndex(a => a == "Programmed Meal")) + 1;

            System.Console.WriteLine($"----------------------------------------------------");
            System.Console.WriteLine(split[total]);
            System.Console.ReadLine();
        }


        private static void TestDmdataMapping()
        {
            MigrationVariables.Init();
            TableAgentCollection.Populate(new List<string> { "PATIENTS", "DMDATA" });

            PatientsMapping pMap = new PatientsMapping();
            pMap.CreatePatientMapping();


            DMDataMapping map = new DMDataMapping();
            map.CreateDMDataMapping();
        }

        private static void TestClinicianMapping()
        {
            var cMap = new ClinicianMapping();
            cMap.CreateClinicianMapping();
        }

        private static void TestInstitutionMapping()
        {
            var iMap = new InstitutionMapping();
            iMap.CreateInstitutionMapping();
        }

        private static void CreateEnums()
        {
            DynamicEnums de = new DynamicEnums();
        }

        private static void TestValidation()
        {
            var val = new InsulinTypeValidation();
            var valid = val.ValidateTable();
            
            if (!valid)
            {
                val.SyncTable();
            }


            //var valid = val.ValidateAll();

            //bool failed = valid.Any(a => a.Value == false);

            //System.Console.WriteLine($"Has any failed: {failed}");
            System.Console.WriteLine(valid);
            System.Console.ReadLine();
        }

        static void TestTransaction()
        {
            var pat = new Patient
            {
                MRID = "12345",
                Firstname = "Mighty",
                Lastname = "Lion",
                Gender = 1,
                DateofBirth = new DateTime(2000, 8, 1)
            };

            var di = new DatabaseInfo { SiteId = 555 };
            TransactionManager.DatabaseContext.Patients.Add(pat);
            TransactionManager.DatabaseContext.DatabaseInfoes.Add(di);
            TransactionManager.ExecuteTransaction();
            //var pCount = TransactionManager.DatabaseContext.Patients.Count();
            //var dCount = TransactionManager.DatabaseContext.DatabaseInfoes.Count();
            //System.Console.WriteLine(pCount);
            //System.Console.WriteLine(dCount);

            //System.Console.ReadLine();
        }

        static void TestPatientTransaction()
        {
            var pMap = new PatientsMapping();

            pMap.CreatePatientMapping();
            TransactionManager.ExecuteTransaction();
            //var pCount = TransactionManager.DatabaseContext.Patients.Count();
            //var dCount = TransactionManager.DatabaseContext.DatabaseInfoes.Count();
            //System.Console.WriteLine(pCount);
            //System.Console.WriteLine(dCount);

            //System.Console.ReadLine();
        }

    }
}
