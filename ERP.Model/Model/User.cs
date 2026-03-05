using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpires { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<PriceList> PriceListCreatedByNavigations { get; set; } = new List<PriceList>();

    public virtual ICollection<PriceListItem> PriceListItemCreatedByNavigations { get; set; } = new List<PriceListItem>();

    public virtual ICollection<PriceListItem> PriceListItemLastUpdatedByNavigations { get; set; } = new List<PriceListItem>();

    public virtual ICollection<PriceListItem> PriceListItemRemovedByNavigations { get; set; } = new List<PriceListItem>();

    public virtual ICollection<PriceList> PriceListLastUpdatedByNavigations { get; set; } = new List<PriceList>();

    public virtual ICollection<PriceList> PriceListRemovedByNavigations { get; set; } = new List<PriceList>();
}
