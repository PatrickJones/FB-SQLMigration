using Newtonsoft.Json;
using NuLibrary.Migration.CustomEvents;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class MeterReadingHandler
    {
        MappingUtilities mu = new MappingUtilities();

        private DataRowCollection DataRows;

        public event EventHandler<MeterReadingHandlerEventArgs> BGExtractionEvent;
        public event EventHandler<MeterReadingHandlerEventArgs> PumpDeliveryExtractionEvent;
        public event EventHandler<MeterReadingHandlerEventArgs> PumpEventsExtractionEvent;
        public event EventHandler<MeterReadingHandlerEventArgs> NutritionExtractionEvent;
        public event EventHandler<MeterReadingHandlerEventArgs> UserSettingsExtractionEvent;

        public ConcurrentBag<BloodGlucoseReading> BloodGlucoseReadings = new ConcurrentBag<BloodGlucoseReading>();
        public ConcurrentBag<NutritionReading> NutritionReadings = new ConcurrentBag<NutritionReading>();
        public ConcurrentBag<ReadingEvent> ReadingEvents = new ConcurrentBag<ReadingEvent>();
        public ConcurrentBag<DeviceSetting> DeviceSettings = new ConcurrentBag<DeviceSetting>();
        public ConcurrentBag<BolusDelivery> BolusDeliveries = new ConcurrentBag<BolusDelivery>();
        public ConcurrentBag<BasalDelivery> BasalDeliveries = new ConcurrentBag<BasalDelivery>();
        public ConcurrentBag<TotalDailyInsulinDelivery> TotalDailyInsulinDeliveries = new ConcurrentBag<TotalDailyInsulinDelivery>();

        public MeterReadingHandler(DataRowCollection dataRows)
        {
            DataRows = dataRows;
            Extract();
        }

        private void Extract()
        {
            DataRow[] rowArray = new DataRow[DataRows.Count];
            DataRows.CopyTo(rowArray, 0);

            var bgs = rowArray.Where(w => string.Equals(w["READINGTYPE"].ToString(), "bg", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var pds = rowArray.Where(w => string.Equals(w["READINGTYPE"].ToString(), "pump delivery", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var pes = rowArray.Where(w => string.Equals(w["READINGTYPE"].ToString(), "pump events", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var nuts = rowArray.Where(w => string.Equals(w["READINGTYPE"].ToString(), "nutrition", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var us = rowArray.Where(w => string.Equals(w["READINGTYPE"].ToString(), "user settings", StringComparison.CurrentCultureIgnoreCase)).ToList();

            var taskList = new List<Task>() {
                Task.Run(() => ExtractBg(bgs)),
                Task.Run(() => ExtractPumpDelivery(pds)),
                Task.Run(() => ExtractPumpEvents(pes)),
                Task.Run(() => ExtractNutrition(nuts)),
                Task.Run(() => ExtractSettings(us)),
            };

            Task.WhenAll(taskList);
        }

        private void ExtractSettings(ICollection<DataRow> rows)
        {
           Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var ds = new DeviceSetting
                    {
                        ReadingKeyId = keyId,
                        Name = (row["EVENTSUBTYPE_1"] is DBNull) ? String.Empty : row["EVENTSUBTYPE_1"].ToString(),
                        Value = (row["CLINIPROVALUE"] is DBNull) ? String.Empty : row["CLINIPROVALUE"].ToString(),
                        UserId = userId
                    };

                    DeviceSettings.Add(ds);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("DEVICESETTINGS", typeof(DeviceSetting), JsonConvert.SerializeObject(row), "Failed to map User Setting reading.");
                }
            });

            OnUserSettingsExtractionEvent(new MeterReadingHandlerEventArgs(true));
        }

        private void NoReadingTypeMatch(DataRow row)
        {
            MappingStatistics.LogFailedMapping("METERREADING", typeof(DataRow), JsonConvert.SerializeObject(row), "Failed to parse METERREADING row.");
        }

        private void ExtractNutrition(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());
                string eventSubType = (row["EVENTSUBTYPE_1"] is DBNull) ? String.Empty : row["EVENTSUBTYPE_1"].ToString();

                if (!keyId.Equals(Guid.Empty))
                {
                    var date = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());

                    var nt = new NutritionReading
                    {
                        ReadingDateTime = date,
                        ReadingKeyId = keyId,
                        UserId = userId,
                        Date = date
                    };

                    bool canAdd = true;
                    var cpValue = (row["CLINIPROVALUE"] is DBNull) ? 0 : mu.ParseDouble(row["CLINIPROVALUE"].ToString());

                    switch (eventSubType)
                    {
                        case "NUT_CARBS":
                            nt.Carbohydrates = cpValue;
                            break;
                        case "NUT_FAT":
                            nt.Fat = cpValue;
                            break;
                        case "NUT_CAL":
                            nt.Calories = cpValue;
                            break;
                        case "NUT_PROT":
                            nt.Protien = cpValue;
                            break;
                        default:
                            canAdd = false;
                            NoReadingTypeMatch(row);
                            break;
                    }

                    if (canAdd)
                    {
                        NutritionReadings.Add(nt);
                    }
                }
                else
                {
                    MappingStatistics.LogFailedMapping("NUTRITIONREADINGS", typeof(NutritionReading), JsonConvert.SerializeObject(row), "Failed to map nutrition reading.");
                }
            });

            OnNutritionExtractionEvent(new MeterReadingHandlerEventArgs(true));
        }

        private void ExtractPumpEvents(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());
                string eventSubType1 = (row["EVENTSUBTYPE_1"] is DBNull) ? String.Empty : row["EVENTSUBTYPE_1"].ToString();

                if (!keyId.Equals(Guid.Empty))
                {
                    var pe = new ReadingEvent
                    {
                        EventTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString()),
                        ReadingKeyId = keyId,
                        EventValue = (row["CLINIPROVALUE"] is DBNull) ? String.Empty : row["CLINIPROVALUE"].ToString(),
                        UserId = userId,
                        StartTime = new DateTime(1800, 1, 1),
                        StopTime = new DateTime(1800, 1, 1),
                        ResumeTime = new DateTime(1800, 1, 1)
                    };

                    switch (eventSubType1.ToLower())
                    {
                        case "activate":
                            pe.EventType = (int)ReadingEventType.Activate;
                            break;
                        case "date_change":
                            pe.EventType = (int)ReadingEventType.DateChange;
                            break;
                        case "deactivate":
                            pe.EventType = (int)ReadingEventType.Deactivate;
                            break;
                        case "remote_hazard":
                            pe.EventType = (int)ReadingEventType.RemoteHazard;
                            break;
                        case "hist_alarm":
                            pe.EventType = (int)ReadingEventType.AlarmHistory;
                            break;
                        case "download":
                            pe.EventType = (int)ReadingEventType.Download;
                            break;
                        case "occlusion":
                            pe.EventType = (int)ReadingEventType.Occlusion;
                            break;
                        case "pump_advise":
                            pe.EventType = (int)ReadingEventType.PumpAdvise;
                            break;
                        case "pump_hazard":
                            pe.EventType = (int)ReadingEventType.PumpHazard;
                            break;
                        case "time_adjust":
                            pe.EventType = (int)ReadingEventType.TimeChange;
                            break;
                        case "suspended":
                            pe.EventType = (int)ReadingEventType.Suspend;
                            break;
                        case "resume":
                            pe.EventType = (int)ReadingEventType.Resume;
                            break;
                        default:
                            pe.EventType = 0;
                            break;
                    }

                    ReadingEvents.Add(pe);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("READINGEVENT", typeof(ReadingEvent), JsonConvert.SerializeObject(row), "Failed to map reading event.");
                }
            });

            OnPumpEventsExtractionEvent(new MeterReadingHandlerEventArgs(true));
        }

        private void ExtractPumpDelivery(ICollection<DataRow> rows)
        {
            var bolus = rows.Where(w => string.Equals(w["EVENTSUBTYPE_1"].ToString(), "bolus", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var basals = rows.Where(w => string.Equals(w["EVENTSUBTYPE_1"].ToString(), "basal", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var tdds = rows.Where(w => string.Equals(w["EVENTSUBTYPE_1"].ToString(), "tdd", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var termba = rows.Where(w => string.Equals(w["EVENTSUBTYPE_1"].ToString(), "term_basal", StringComparison.CurrentCultureIgnoreCase)).ToList();
            var termbo = rows.Where(w => string.Equals(w["EVENTSUBTYPE_1"].ToString(), "term_bolus", StringComparison.CurrentCultureIgnoreCase)).ToList();

            var taskList = new List<Task>() {
                Task.Run(() => ExtractBolus(bolus)),
                Task.Run(() => ExtractBasal(basals)),
                Task.Run(() => ExtractTdd(tdds)),
                Task.Run(() => ExtractTermBasal(termba)),
                Task.Run(() => ExtractTermBolus(termbo)),
            };

            Task.WhenAll(taskList).ContinueWith(done => OnPumpDeliveryExtractionEvent(new MeterReadingHandlerEventArgs(true)));
        }

        private void ExtractTermBolus(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var bd = new BolusDelivery();
                    var dd = new BolusDeliveryData();

                    dd.Name = "TERM_BOLUS";
                    dd.Value = (row["READINGNOTE"] is DBNull) ? String.Empty : row["READINGNOTE"].ToString();
                    dd.Date = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());

                    bd.ReadingKeyId = keyId;
                    bd.UserId = userId;
                    bd.StartDateTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());
                    bd.AmountDelivered = 0;
                    bd.AmountSuggested = 0;
                    bd.Duration = 0;
                    bd.Type = "BolusDeliveryData";
                    bd.BolusDeliveryDatas.Add(dd);

                    BolusDeliveries.Add(bd);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("BOLUSDELIVERY", typeof(BolusDelivery), JsonConvert.SerializeObject(row), "Failed to map TERM_BOLUS reading.");
                }
            });
        }

        private void ExtractTermBasal(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var bd = new BasalDelivery();
                    var dd = new BasalDeliveryData();

                    dd.Name = "TERM_BASAL";
                    dd.Value = (row["READINGNOTE"] is DBNull) ? String.Empty : row["READINGNOTE"].ToString();
                    dd.Date = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());

                    bd.ReadingKeyId = keyId;
                    bd.UserId = userId;
                    bd.StartDateTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());
                    bd.AmountDelivered = 0;
                    bd.DeliveryRate = 0;
                    bd.Duration = String.Empty;
                    bd.IsTemp = false;
                    bd.BasalDeliveryDatas.Add(dd);

                    BasalDeliveries.Add(bd);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("BASALDELIVERY", typeof(BasalDelivery), JsonConvert.SerializeObject(row), "Failed to map TERM_BASAL reading.");
                }
            });
        }

        private void ExtractTdd(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var bolus = row["READINGNOTE"].ToString().Split().ElementAt(0).Split('=').LastOrDefault();
                    var bolusConf = row["READINGNOTE"].ToString().Split().ElementAt(1).Split('=').LastOrDefault();
                    var basal = row["READINGNOTE"].ToString().Split().ElementAt(3).Split('=').LastOrDefault();
                    var basalConf = row["READINGNOTE"].ToString().Split().ElementAt(4).Split('=').LastOrDefault();
                    var total = row["READINGNOTE"].ToString().Split().ElementAt(6).Split('=').LastOrDefault();

                    bool ptGood = Double.TryParse(total, out double pt);
                    bool puGood = Double.TryParse(bolus, out double pu);
                    bool paGood = Double.TryParse(basal, out double pa);

                    var td = new TotalDailyInsulinDelivery
                    {
                        TotalDelivered = (ptGood) ? pt : 0,
                        BasalDelivered = (paGood) ? pa : 0,
                        Suspended = false,
                        TempActivated = false,
                        Valid = (string.Equals(bolusConf, "t", StringComparison.CurrentCultureIgnoreCase) && string.Equals(basalConf, "t", StringComparison.CurrentCultureIgnoreCase)) ? true : false,
                        BolusDelivered = (puGood) ? pu : 0,
                        Date = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString()),
                        ReadingKeyId = keyId,
                        UserId = userId
                    };

                    TotalDailyInsulinDeliveries.Add(td);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("TOTALDAILYINSULINDELIVERY", typeof(TotalDailyInsulinDelivery), JsonConvert.SerializeObject(row), "Failed to map TotalDailyInsulinDelivery reading.");
                }
            });
        }

        private void ExtractBasal(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var value = (row["CLINIPROVALUE"] is DBNull) ? String.Empty : row["CLINIPROVALUE"].ToString();

                    bool ptGood = Double.TryParse(value, out double rate);

                    var bd = new BasalDelivery();

                    bd.ReadingKeyId = keyId;
                    bd.UserId = userId;
                    bd.StartDateTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());
                    bd.AmountDelivered = 0; // 0 means you must group times and multiple by rate programmaically
                    bd.DeliveryRate = rate;
                    bd.Duration = String.Empty;
                    bd.IsTemp = false;

                    BasalDeliveries.Add(bd);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("BASALDELIVERY", typeof(BasalDelivery), JsonConvert.SerializeObject(row), "Failed to map BASAL reading.");
                }
            });
        }

        private void ExtractBolus(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());
                DateTime date = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    Dictionary<string, string> bolusData = GetBolusData(row["METERSENT"].ToString());


                    var carb = new BolusCarb
                    {
                        Date = date,
                        CarbValue = mu.ParseInt(bolusData["Carbs"])
                    };

                    var bgTar = new BGTarget
                    {
                        Date = date,
                        TargetBG = mu.ParseInt(bolusData["Target BG"])
                    };

                    var ic = new InsulinCarbRatio
                    {
                        Date = date,
                        ICRatio = mu.ParseInt(bolusData["IC Ratio"])
                    };

                    var cf = new CorrectionFactor
                    {
                        Date = date,
                        CorrectionFactorValue = mu.ParseInt(bolusData["Correct"])
                    };

                    var iCorr = new InsulinCorrection
                    {
                        Date = date,
                        InsulinCorrectionValue = mu.ParseInt(bolusData["Correction"]),
                        InsulinCorrectionAbove = mu.ParseInt(bolusData["Correction Above"])
                    };

                    var bd = new BolusDelivery();

                    double dCarbs = mu.ParseDouble(bolusData["Carbs"]);
                    double dIC = mu.ParseDouble(bolusData["IC Ratio"]);

                    bd.ReadingKeyId = keyId;
                    bd.UserId = userId;
                    bd.StartDateTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString());
                    bd.AmountDelivered = mu.ParseDouble(bolusData["Total"]);
                    bd.AmountSuggested = (dCarbs == 0.0) ? 0.0 : (dCarbs / dIC);
                    bd.Duration = mu.ParseInt(bolusData["Extended Duration"]);
                    bd.Type = "BolusDeliveryData";


                    bolusData.Remove("Carbs");
                    bolusData.Remove("Target BG");
                    bolusData.Remove("IC Ratio");
                    bolusData.Remove("Correct");
                    bolusData.Remove("Correction");
                    bolusData.Remove("Correct Above");
                    for (int i = 0; i < bolusData.Count; i++)
                    {
                        bd.BolusDeliveryDatas.Add(new BolusDeliveryData
                        {
                            Date = date,
                            Name = bolusData.ElementAt(i).Key,
                            Value = (String.IsNullOrEmpty(bolusData.ElementAt(i).Value)) ? String.Empty : bolusData.ElementAt(i).Value.Trim()
                        });
                    }

                    bd.BGTarget = bgTar;
                    bd.BolusCarb = carb;
                    bd.InsulinCarbRatio = ic;
                    bd.InsulinCorrection = iCorr;
                    bd.CorrectionFactor = cf;

                    BolusDeliveries.Add(bd);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("BOLUSDELIVERY", typeof(BolusDelivery), JsonConvert.SerializeObject(row), "Failed to map BOLUS reading.");
                }
            });
        }

        private Dictionary<string, string> GetBolusData(string row)
        {
            var results = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(row))
            {
                return results;
            }
            else
            {
                var split = row.Split('\t').ToList();

                int total = (split.FindIndex(a => a == "Total")) + 1;
                results.Add("Total", split[total]);

                int carbs = (split.FindIndex(a => a == "Carbs")) + 1;
                results.Add("Carbs", split[carbs]);

                int icRatio = (split.FindIndex(a => a == "IC Ratio")) + 1;
                results.Add("IC Ratio", split[icRatio]);

                int targetBg = (split.FindIndex(a => a == "Target BG")) + 1;
                results.Add("Target BG", split[targetBg]);

                int actualBg = (split.FindIndex(a => a == "BG")) + 1;
                results.Add("BG", split[actualBg]);

                int correctAbove = (split.FindIndex(a => a == "Correction Above")) + 1;
                results.Add("Correction Above", split[correctAbove]);

                int correct = (split.FindIndex(a => a == "Correct")) + 1;
                results.Add("Correct", split[correct]);

                int volume = (split.FindIndex(a => a == "Volume")) + 1;
                results.Add("Volume", split[volume]);

                int imDuration = (split.FindIndex(a => a == "Immediate Duration")) + 1;
                results.Add("Immediate Duration", split[imDuration]);

                int exDuration = (split.FindIndex(a => a == "Extended Duration")) + 1;
                results.Add("Extended Duration", split[exDuration]);

                int correction = (split.FindIndex(a => a == "Correction")) + 1;
                results.Add("Correction", split[correction]);

                int correctionIOB = (split.FindIndex(a => a == "Correction Insulin on Board")) + 1;
                results.Add("Correction Insulin on Board", split[correctionIOB]);

                int mealIOB = (split.FindIndex(a => a == "Meal Insulin on Board")) + 1;
                results.Add("Meal Insulin on Board", split[mealIOB]);

                int revCorrection = (split.FindIndex(a => a == "Reverse Correction")) + 1;
                results.Add("Reverse Correction", split[revCorrection]);

                int progCorrection = (split.FindIndex(a => a == "Programmed Correction")) + 1;
                results.Add("Programmed Correction", split[progCorrection]);

                int progMeal = (split.FindIndex(a => a == "Programmed Meal")) + 1;
                results.Add("Programmed Meal", split[progMeal]);

                return results;
            }
        }

        private void ExtractBg(ICollection<DataRow> rows)
        {
            Parallel.ForEach(rows, row => {
                Guid keyId = MemoryMappings.GetReadingHeaderKeyId(row["DOWNLOADKEYID"].ToString());
                Guid userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, row["PATIENTKEYID"].ToString());

                if (!keyId.Equals(Guid.Empty))
                {
                    var bg = new BloodGlucoseReading
                    {
                        Active = true,
                        ReadingDateTime = (row["READINGDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["READINGDATETIME"].ToString()),
                        ReadingKeyId = keyId,
                        Units = (row["METERUNITS"] is DBNull) ? String.Empty : row["METERUNITS"].ToString(),
                        Value = (row["CLINIPROVALUE"] is DBNull) ? String.Empty : row["CLINIPROVALUE"].ToString(),
                        UserId = userId
                    };

                    BloodGlucoseReadings.Add(bg);
                }
                else
                {
                    MappingStatistics.LogFailedMapping("BLOODGLUCOSEREADING", typeof(BloodGlucoseReading), JsonConvert.SerializeObject(row), "Failed to map BG reading.");
                }
            });

            OnBGExtractionEvent(new MeterReadingHandlerEventArgs(true));
        }

        #region Event Raisers
        protected virtual void OnBGExtractionEvent(MeterReadingHandlerEventArgs e)
        {
            EventHandler<MeterReadingHandlerEventArgs> eHandler = BGExtractionEvent;
            if (eHandler != null)
            {
                e.ExtractionName = "BG Extraction";
                e.ExtractionSuccessful = true;

                eHandler(this, e);
            }
        }

        protected virtual void OnPumpDeliveryExtractionEvent(MeterReadingHandlerEventArgs e)
        {
            EventHandler<MeterReadingHandlerEventArgs> eHandler = PumpDeliveryExtractionEvent;
            if (eHandler != null)
            {
                e.ExtractionName = "Pump Delivery Extraction";
                e.ExtractionSuccessful = true;

                eHandler(this, e);
            }
        }

        protected virtual void OnPumpEventsExtractionEvent(MeterReadingHandlerEventArgs e)
        {
            EventHandler<MeterReadingHandlerEventArgs> eHandler = PumpEventsExtractionEvent;
            if (eHandler != null)
            {
                e.ExtractionName = "Pump Events Extraction";
                e.ExtractionSuccessful = true;

                eHandler(this, e);
            }
        }

        protected virtual void OnNutritionExtractionEvent(MeterReadingHandlerEventArgs e)
        {
            EventHandler<MeterReadingHandlerEventArgs> eHandler = NutritionExtractionEvent;
            if (eHandler != null)
            {
                e.ExtractionName = "Nutrition Extraction";
                e.ExtractionSuccessful = true;

                eHandler(this, e);
            }
        }

        protected virtual void OnUserSettingsExtractionEvent(MeterReadingHandlerEventArgs e)
        {
            EventHandler<MeterReadingHandlerEventArgs> eHandler = UserSettingsExtractionEvent;
            if (eHandler != null)
            {
                e.ExtractionName = "User Settings Extraction";
                e.ExtractionSuccessful = true;

                eHandler(this, e);
            }
        }

        #endregion
    }
}
