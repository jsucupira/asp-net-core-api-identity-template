using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace TemplateProject.Common
{
    public static class Extensions
    {
        public static PagedList<T> CreatePagination<T>(this IQueryable<T> queryable, SearchParameters searchParameters)
        {
            IQueryable<T> orderable = queryable;

            if (!string.IsNullOrEmpty(searchParameters.OrderBy))
                orderable = orderable.OrderBy(searchParameters.OrderBy);

            if (!string.IsNullOrEmpty(searchParameters.OrderByDesc))
                orderable = orderable.OrderBy(searchParameters.OrderByDesc + " DESC");

            if (!string.IsNullOrEmpty(searchParameters.SearchQuery) && !string.IsNullOrEmpty(searchParameters.SearchParameter))
            {
                orderable = orderable.Where($"{searchParameters.SearchParameter}.ToLowerInvariant().Contains(@0)", searchParameters.SearchQuery.ToLowerInvariant());
            }

            return PagedList<T>.Create(orderable, searchParameters.PageNumber, searchParameters.PageSize);
        }

        public static object CreateResourceUri<T>(this PagedList<T> pagedList, SearchParameters searchParameters, IUrlHelper urlHelper, string getName)
        {
            string previousPageLink = pagedList.HasPrevious
                ? urlHelper.Link(getName, new
                {
                    orderBy = searchParameters.OrderBy,
                    searchQuery = searchParameters.SearchQuery,
                    searchParameters.SearchParameter,
                    pageNumber = searchParameters.PageNumber - 1,
                    pageSize = searchParameters.PageSize
                })
                : null;

            string nextPageLink = pagedList.HasNext
                ? urlHelper.Link(getName,
                    new
                    {
                        orderBy = searchParameters.OrderBy,
                        searchQuery = searchParameters.SearchQuery,
                        searchParameters.SearchParameter,
                        pageNumber = searchParameters.PageNumber + 1,
                        pageSize = searchParameters.PageSize
                    })
                : null;

            return new
            {
                previousPageLink,
                nextPageLink,
                totalCount = pagedList.TotalCount,
                pageSize = pagedList.PageSize,
                currentPage = pagedList.CurrentPage,
                totalPages = pagedList.TotalPages
            };
        }

        public static bool PropertyExists<T>(string propName)
        {
            Type type = typeof(T);
            propName = propName?.Trim();

            if (propName != null)
            {
                PropertyInfo property = type.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                return property != null;
            }
            return false;
        }
    }
}