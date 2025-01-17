using Application.Models.DTOs.Common;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Helpers
{
    public class Logs
    {
        public AuditLog SaveLog(string moduleName, string actionType, string actionDesc, string actionBy)
        {
            AuditLog audit = new AuditLog();
            audit.ModuleName = moduleName;
            audit.ActionType = actionType;
            audit.ActionDesc = actionDesc;
            audit.ActionBy = actionBy;
            audit.DateEntry = DateTime.Now;

            return audit;
        }
    }
}
