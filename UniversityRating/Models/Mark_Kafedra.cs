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
    
    public partial class Mark_Kafedra
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mark_Kafedra()
        {
            this.Status_Doc_Kafedra = new HashSet<Status_Doc_Kafedra>();
        }
    
        public int Id { get; set; }
        public int Id_Kafedra { get; set; }
        public int Id_Criteria { get; set; }
        public int Kolvo_ed { get; set; }
        public Nullable<int> Status { get; set; }
        public int Kolvo_Mark { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    
        public virtual Criteria_Kafedra Criteria_Kafedra { get; set; }
        public virtual Kafedra Kafedra { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Doc_Kafedra> Status_Doc_Kafedra { get; set; }
    }
}
