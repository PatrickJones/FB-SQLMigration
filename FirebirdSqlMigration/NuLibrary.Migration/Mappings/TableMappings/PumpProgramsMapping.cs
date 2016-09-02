﻿using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PumpProgramsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpProgramsMapping() : base("PumpPrograms")
        {

        }

        public PumpProgramsMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePumpsMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                

                var CreationDate = (DateTime)row["CREATEDATE"];
                var Source = (String)row["SOURCE"];
                var Valid = (Boolean)row["ACTIVEPROGRAM"];


                for (int i = 1; i < 7; i++)
                {
                    PumpProgram p = new PumpProgram();

                    p.CreationDate = CreationDate;
                    p.Source = Source;
                    p.Valid = Valid;

                    p.ProgramKey = (Int32)row[$"PROG{i}KEYID"];

                    TransactionManager.DatabaseContext.PumpPrograms.Add(p);
                }
                   
                //Example Output:
                //ProgramKey = (Int32)row["PROG3KEYID"]
                //ProgramKey = (Int32)row["PROG4KEYID"]
                //ProgramKey = (Int32)row["PROG5KEYID"]
                //ProgramKey = (Int32)row["PROG6KEYID"]
                //ProgramKey = (Int32)row["PROG7KEYID"]
                
            }
        }
    }
}