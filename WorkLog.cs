//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kadr
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkLog
    {
        public int WorkLogID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Project Project { get; set; }
    }
}
