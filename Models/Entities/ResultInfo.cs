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
    
    public partial class ResultInfo
    {
        public int IDResult { get; set; }
        public int IDQuestion { get; set; }
        public int IDAnswer { get; set; }
    
        public virtual Answer Answer { get; set; }
        public virtual Question Question { get; set; }
        public virtual Result Result { get; set; }
    }
}