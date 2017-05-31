//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ReadingHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReadingHeader()
        {
            this.BasalDeliveries = new HashSet<BasalDelivery>();
            this.ReadingErrors = new HashSet<ReadingError>();
            this.ReadingEvents = new HashSet<ReadingEvent>();
            this.TotalDailyInsulinDeliveries = new HashSet<TotalDailyInsulinDelivery>();
            this.DeviceSettings = new HashSet<DeviceSetting>();
            this.BolusDeliveries = new HashSet<BolusDelivery>();
            this.BloodGlucoseReadings = new HashSet<BloodGlucoseReading>();
            this.NutritionReadings = new HashSet<NutritionReading>();
            this.PhysiologicalReadings = new HashSet<PhysiologicalReading>();
            this.TensReadings = new HashSet<TensReading>();
        }
    
        public int DeviceId { get; set; }
        public System.DateTime ServerDateTime { get; set; }
        public System.DateTime MeterDateTime { get; set; }
        public int Readings { get; set; }
        public string SiteSource { get; set; }
        public System.DateTime ReviewedOn { get; set; }
        public bool IsCGMData { get; set; }
        public System.Guid UserId { get; set; }
        public string LegacyDownloadKeyId { get; set; }
        public System.Guid ReadingKeyId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BasalDelivery> BasalDeliveries { get; set; }
        public virtual PatientDevice PatientDevice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReadingError> ReadingErrors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReadingEvent> ReadingEvents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TotalDailyInsulinDelivery> TotalDailyInsulinDeliveries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeviceSetting> DeviceSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BolusDelivery> BolusDeliveries { get; set; }
        public virtual Pump Pump { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BloodGlucoseReading> BloodGlucoseReadings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NutritionReading> NutritionReadings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhysiologicalReading> PhysiologicalReadings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TensReading> TensReadings { get; set; }
    }
}
