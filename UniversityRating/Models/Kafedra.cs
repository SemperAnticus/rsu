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
    
    public partial class Kafedra
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kafedra()
        {
            this.Mark_Kafedra = new HashSet<Mark_Kafedra>();
            this.Teachers = new HashSet<Teacher>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Id_Facility { get; set; }
        public Nullable<int> TotalMark { get; set; }
        public bool Is_Released { get; set; }
        public Nullable<int> Id_TeacherZav { get; set; }
        public string SName { get; set; }
        public int MarkB1 { get; set; }
        public int MarkB2 { get; set; }
        public int MarkB3 { get; set; }
        public int MarkB4 { get; set; }
        public int MarkB5 { get; set; }
    
        public virtual Facility Facility { get; set; }
        public virtual Teacher Teacher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mark_Kafedra> Mark_Kafedra { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
