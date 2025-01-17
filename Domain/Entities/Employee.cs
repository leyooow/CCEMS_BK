using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }
}
