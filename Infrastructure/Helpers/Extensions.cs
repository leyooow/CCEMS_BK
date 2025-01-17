using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class Extensions
    {
        //public static string GetEnumDisplayName(this Enum enumType)
        //{
        //    return enumType.GetType().GetMember(enumType.ToString())
        //           .First()
        //           .GetCustomAttribute<DisplayAttribute>()
        //           .Name;
        //}

        /// <summary>
        /// Gets human-readable version of enum.
        /// </summary>
        /// <returns>effective DisplayAttribute.Name of given enum.</returns>
        public static string GetDisplayName<T>(this T enumValue) where T : IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Argument must be of type Enum");

            DisplayAttribute displayAttribute = enumValue.GetType()
                                                         .GetMember(enumValue.ToString())
                                                         .First()
                                                         .GetCustomAttribute<DisplayAttribute>();

            string displayName = displayAttribute?.GetName();

            return displayName ?? enumValue.ToString();
        }
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            var dataTable = new DataTable();

            // If the list is empty, return an empty DataTable
            if (list == null || list.Count == 0)
            {
                return dataTable;
            }

            // Get the properties of the first item in the list (assuming all items have the same properties)
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add columns to DataTable based on the properties
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Add rows to the DataTable
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value; // Handling null values
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
