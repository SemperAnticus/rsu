//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniversityRating.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mark_Teachers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mark_Teachers()
        {
            this.MarkTeachersChecks = new HashSet<MarkTeachersCheck>();
            this.Status_Doc_Teacher = new HashSet<Status_Doc_Teacher>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Id_teachers { get; set; }
        public Nullable<int> Id_Criteria { get; set; }
        public Nullable<int> Kolvo_ed { get; set; }
        public Nullable<int> Status { get; set; }
        public int Kolvo_Mark { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    
        public virtual Teacher Teacher { get; set; }
        public virtual Сriteria_Teachers Сriteria_Teachers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MarkTeachersCheck> MarkTeachersChecks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Doc_Teacher> Status_Doc_Teacher { get; set; }
    }
}
