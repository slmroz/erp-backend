using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Customer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TaxId { get; set; }

    public string? Address { get; set; }

    public string? ZipCode { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Www { get; set; }

    public string? Facebook { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
