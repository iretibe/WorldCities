using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace WorldCities.Angular.Data
{
    public class ApiResult<T>
    {
        //The data result
        public List<T> Data { get; private set; }

        //Zero-based index of current page
        public int PageIndex { get; private set; }

        //Number of items contained in each page.
        public int PageSize { get; private set; }

        //Total items count
        public int TotalCount { get; private set; }

        //Total pages count
        public int TotalPages { get; private set; }

        //TRUE if the current page has a previous page, FALSE otherwise.
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        //TRUE if the current page has a next page, FALSE otherwise.
        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }

        //Sorting Column name (or null if none set)
        public string SortColumn { get; set; }

        //Sorting Order ("ASC", "DESC" or null if none set)
        public string SortOrder { get; set; }
        
        //Filter Column name(or null if none set)
        public string FilterColumn { get; set; }
        
        //Filter Query string (to be used within the given FilterColumn)
        public string FilterQuery { get; set; }


        //Private constructor called by the CreateAsync method.
        private ApiResult(List<T> data, int count, int pageIndex, int pageSize, 
            string sortColumn, string sortOrder, string filterColumn, string filterQuery)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }

        //Checks if the given property name exists to protect against SQL injection attacks
        public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, 
            int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null,
            string filterQuery = null)
        {
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
            {
                source = source.Where(
                    string.Format("{0}.Contains(@0)",
                    filterColumn),
                    filterQuery);
            }

            var count = await source.CountAsync();

            if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder)
                    && sortOrder.ToUpper() == "ASC"
                    ? "ASC"
                    : "DESC";

                source = source.OrderBy(
                    string.Format(
                        "{0} {1}",
                        sortColumn,
                        sortOrder));
            }

            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            //Retrieve the SQL query (for debug purposes)
            {
                var sql = source.ToParametrizedSql();
                // do something with the sql string
            }

            var data = await source.ToListAsync();

            return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);

            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(
                    string.Format(
                        "ERROR: Property '{0}' does not exist.",
                        propertyName));

            return prop != null;
        }
    }
}
