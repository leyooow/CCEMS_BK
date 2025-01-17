using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Application.Models.DTOs.Report
{
    public class ReportDTO : Infrastructure.Entities.Report
    {
        public string StatusName
        {
            get
            {
                return Enum.GetName(typeof(ReportStatus), Status);
            }
            set
            {
                if (Enum.TryParse(typeof(ReportStatus), value, out var enumValue))
                {
                    Status = (int)enumValue;
                }
            }
        }
        public string ReportCoverageName
        {
            get
            {
                return Enum.GetName(typeof(ReportCoverage), ReportCoverage);
            }
            set
            {
                if (Enum.TryParse(typeof(ReportCoverage), value, out var enumValue))
                {
                    ReportCoverage = (int)enumValue;
                }
            }
        }
        public string ReportCategoryName
        {
            get
            {
                // Convert the int to the enum and get the Display name
                if (Enum.IsDefined(typeof(ReportCategory), ReportCategory))
                {
                    return GetEnumDisplayName((ReportCategory)ReportCategory);
                }
                return string.Empty; // Return empty if the int does not match any enum value
            }
            set
            {
                // Try to set the ReportCategory based on the Display name
                var category = GetEnumFromDisplayName(value);
                if (category != null)
                {
                    ReportCategory = (int)category;
                }
            }
        }


        private string GetEnumDisplayName(ReportCategory category)
        {
            // Get the field info of the enum
            var field = category.GetType().GetField(category.ToString());

            // Get the Display attribute if it exists
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));

            // Return the Display name or the enum's name if the Display attribute is not present
            return attribute != null ? attribute.Name : category.ToString();
        }

        private ReportCategory? GetEnumFromDisplayName(string displayName)
        {
            foreach (ReportCategory category in Enum.GetValues(typeof(ReportCategory)))
            {
                var field = category.GetType().GetField(category.ToString());
                var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));

                if (attribute != null && attribute.Name == displayName)
                {
                    return category;
                }
            }
            return null; // Return null if no matching Display name is found
        }
    }
}
