﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuLibrary.Migration.SQLDatabase.EF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class NuMedicsGlobalEntities : DbContext
    {
        public NuMedicsGlobalEntities()
            : base("name=NuMedicsGlobalEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<AppLoginHistory> AppLoginHistories { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<AppUserSetting> AppUserSettings { get; set; }
        public virtual DbSet<AssignedUserType> AssignedUserTypes { get; set; }
        public virtual DbSet<BasalDelivery> BasalDeliveries { get; set; }
        public virtual DbSet<BasalDeliveryData> BasalDeliveryDatas { get; set; }
        public virtual DbSet<BGTarget> BGTargets { get; set; }
        public virtual DbSet<BloodGlucoseReading> BloodGlucoseReadings { get; set; }
        public virtual DbSet<BolusCarb> BolusCarbs { get; set; }
        public virtual DbSet<BolusDelivery> BolusDeliveries { get; set; }
        public virtual DbSet<BolusDeliveryData> BolusDeliveryDatas { get; set; }
        public virtual DbSet<CareSetting> CareSettings { get; set; }
        public virtual DbSet<CGMReminder> CGMReminders { get; set; }
        public virtual DbSet<CGMSession> CGMSessions { get; set; }
        public virtual DbSet<Check> Checks { get; set; }
        public virtual DbSet<CheckStatu> CheckStatus { get; set; }
        public virtual DbSet<Clinician> Clinicians { get; set; }
        public virtual DbSet<CorrectionFactor> CorrectionFactors { get; set; }
        public virtual DbSet<DailyTimeSlot> DailyTimeSlots { get; set; }
        public virtual DbSet<DatabaseInfo> DatabaseInfoes { get; set; }
        public virtual DbSet<DataShareCategory> DataShareCategories { get; set; }
        public virtual DbSet<DataShareRequestLog> DataShareRequestLogs { get; set; }
        public virtual DbSet<DeviceData> DeviceDatas { get; set; }
        public virtual DbSet<DeviceSetting> DeviceSettings { get; set; }
        public virtual DbSet<DiabetesControlType> DiabetesControlTypes { get; set; }
        public virtual DbSet<DiabetesManagementData> DiabetesManagementDatas { get; set; }
        public virtual DbSet<EndUserLicenseAgreement> EndUserLicenseAgreements { get; set; }
        public virtual DbSet<InstitutionAddress> InstitutionAddresses { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<InsulinBrand> InsulinBrands { get; set; }
        public virtual DbSet<InsulinCarbRatio> InsulinCarbRatios { get; set; }
        public virtual DbSet<InsulinCorrection> InsulinCorrections { get; set; }
        public virtual DbSet<InsulinMethod> InsulinMethods { get; set; }
        public virtual DbSet<InsulinType> InsulinTypes { get; set; }
        public virtual DbSet<InsuranceAddress> InsuranceAddresses { get; set; }
        public virtual DbSet<InsuranceContact> InsuranceContacts { get; set; }
        public virtual DbSet<InsurancePlan> InsurancePlans { get; set; }
        public virtual DbSet<InsuranceProvider> InsuranceProviders { get; set; }
        public virtual DbSet<LinkType> LinkTypes { get; set; }
        public virtual DbSet<MedicalRecordIdentifier> MedicalRecordIdentifiers { get; set; }
        public virtual DbSet<NuMedicsUserPrintSetting> NuMedicsUserPrintSettings { get; set; }
        public virtual DbSet<NutritionReading> NutritionReadings { get; set; }
        public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
        public virtual DbSet<PatientAddress> PatientAddresses { get; set; }
        public virtual DbSet<PatientDevice> PatientDevices { get; set; }
        public virtual DbSet<PatientInstitutionLinkHistory> PatientInstitutionLinkHistories { get; set; }
        public virtual DbSet<PatientMedication> PatientMedications { get; set; }
        public virtual DbSet<PatientPhoneNumber> PatientPhoneNumbers { get; set; }
        public virtual DbSet<PatientPhoto> PatientPhotos { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PayPal> PayPals { get; set; }
        public virtual DbSet<PhysiologicalReading> PhysiologicalReadings { get; set; }
        public virtual DbSet<ProgramTimeSlot> ProgramTimeSlots { get; set; }
        public virtual DbSet<PumpProgram> PumpPrograms { get; set; }
        public virtual DbSet<PumpProgramType> PumpProgramTypes { get; set; }
        public virtual DbSet<Pump> Pumps { get; set; }
        public virtual DbSet<PumpSetting> PumpSettings { get; set; }
        public virtual DbSet<ReadingError> ReadingErrors { get; set; }
        public virtual DbSet<ReadingEvent> ReadingEvents { get; set; }
        public virtual DbSet<ReadingEventType> ReadingEventTypes { get; set; }
        public virtual DbSet<ReadingHeader> ReadingHeaders { get; set; }
        public virtual DbSet<SharedArea> SharedAreas { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public virtual DbSet<TensReading> TensReadings { get; set; }
        public virtual DbSet<TherapyType> TherapyTypes { get; set; }
        public virtual DbSet<TotalDailyInsulinDelivery> TotalDailyInsulinDeliveries { get; set; }
        public virtual DbSet<UserAuthentication> UserAuthentications { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<EmailUsernameView> EmailUsernameViews { get; set; }
        public virtual DbSet<PatientListView> PatientListViews { get; set; }
        public virtual DbSet<SubscriptionsView> SubscriptionsViews { get; set; }
        public virtual DbSet<TableRowCount> TableRowCounts { get; set; }
    
        public virtual int CreateTestClinician(string username, string email, Nullable<System.Guid> institutionId)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("Username", username) :
                new ObjectParameter("Username", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var institutionIdParameter = institutionId.HasValue ?
                new ObjectParameter("InstitutionId", institutionId) :
                new ObjectParameter("InstitutionId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CreateTestClinician", usernameParameter, emailParameter, institutionIdParameter);
        }
    }
}
