//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SecurityTest.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Result
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Result()
        {
            this.ResultInfoes = new HashSet<ResultInfo>();
        }
    
        public int ID { get; set; }
        public int IDUser { get; set; }
        public int IDTest { get; set; }
        public Nullable<int> TimeInSecond { get; set; }
    
        public virtual Test Test { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultInfo> ResultInfoes { get; set; }
    }
}
