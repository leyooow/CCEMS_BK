using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models.DTOs.User.role;

public class RolePermissionDTO
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string? Permission { get; set; }
}

public class AddPermissionRequest
{
    public int RoleId { get; set; }
    public List<string>? PermissionList { get; set; }
}