using System;
using System.Collections.Generic;

namespace TaskWebApi.Models;

public partial class TvProduct
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? Product { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
