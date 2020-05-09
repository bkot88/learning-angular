using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace WorldCities.Data
{
    public class ApiResult<T>
    {
        /// <summary>
        /// Private constructor called by the CreateAsync method.
        /// </summary>
        private ApiResult(
            List<T> data,
            int count,
            int pageIndex,
            int pageSize,
            string sortColumn,
            string sortOrder)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Pages a IQueryable source.
        /// </summary>
        /// <param name="source">An IQueryable source of generic
        /// type</param>
        /// <param name="pageIndex">Zero-based current page index
        /// (0 = first page)</param>
        /// <param name="pageSize">The actual size of each
        /// page</param>
        /// <returns>
        /// A object containing the paged result
        /// and all the relevant paging navigation info.
        /// </returns>
        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string sortColumn = null,
            string sortOrder = null)
        {
            var count = await source.CountAsync();
            List<T> data;

            if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "DESC"
                     ? "DESC"
                     : "ASC";
                     
                data = await source
                    .OrderBy($"{sortColumn} {sortOrder}")
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                data = await source
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder);
        }

        private static bool IsValidProperty(string sortColumn)
        {
            var t = typeof(T);
            var prop = t.GetProperty(sortColumn, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            
            
            return prop != null;
        }
            

        /// <summary>
        /// TRUE if the current page has a previous page,
        /// FALSE otherwise.
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }
        /// <summary>
        /// TRUE if the current page has a next page, FALSE otherwise.
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }

        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public string SortColumn { get; }
        public string SortOrder { get; }
        public List<T> Data { get; }
    }
}
