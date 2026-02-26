using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Currency
{
    public int Id { get; set; }

    public string? TargetCurrency { get; set; }

    public string? BaseCurrency { get; set; }

    public decimal Rate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
}
