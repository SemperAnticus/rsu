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
    
    public partial class Category_Teachers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category_Teachers()
        {
            this.Сriteria_Teachers = new HashSet<Сriteria_Teachers>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Id_block { get; set; }
    
        public virtual Block_Teachers Block_Teachers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Сriteria_Teachers> Сriteria_Teachers { get; set; }
    }
}
