using System;
using System.Collections.Generic;

namespace TaskWebApi.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }
}
