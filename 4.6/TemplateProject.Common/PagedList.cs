using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateProject.Common
{
    public class PagedList<T> : List<T>
    {
        private PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            AddRange(items);
        }

        public int CurrentPage { get; private set; }

        public bool HasNext
        {
            get { return (CurrentPage < TotalPages); }
        }

        public bool HasPrevious
        {
            get { return (CurrentPage > 1); }
        }

        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}