using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class ProductGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
