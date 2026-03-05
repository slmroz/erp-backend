using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class PriceList
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? RemovedAt { get; set; }

    public int? RemovedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public int? LastUpdatedBy { get; set; }

    public int CurrencyId { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();

    public virtual User? RemovedByNavigation { get; set; }
}
