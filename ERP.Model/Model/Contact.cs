using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Contact
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNo { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
