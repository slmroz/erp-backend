using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Product
{
    public int Id { get; set; }

    public int ProductGroupId { get; set; }

    public string PartNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Oembrand { get; set; }

    public decimal? ListPrice { get; set; }

    public decimal? WeightKg { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual ProductGroup ProductGroup { get; set; } = null!;
}
