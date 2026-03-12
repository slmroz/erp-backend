using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Priority
{
    public int Id { get; set; }

    public string? Label { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
