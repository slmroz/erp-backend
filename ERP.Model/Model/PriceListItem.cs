using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class PriceListItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? RemovedAt { get; set; }

    public int? RemovedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public int? LastUpdatedBy { get; set; }

    public int PriceListId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual PriceList PriceList { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? RemovedByNavigation { get; set; }
}
