using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Lead
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? ContactId { get; set; }

    public string? Subject { get; set; }

    public string? Description { get; set; }

    public int LeadSourceId { get; set; }

    public int StatusId { get; set; }

    public int PriorityId { get; set; }

    public decimal? EstimatedValue { get; set; }

    public DateOnly? ExpectedResponseDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? RemovedAt { get; set; }

    public int? RemovedBy { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual LeadSource LeadSource { get; set; } = null!;

    public virtual Priority Priority { get; set; } = null!;

    public virtual User? RemovedByNavigation { get; set; }

    public virtual LeadStatus Status { get; set; } = null!;
}
