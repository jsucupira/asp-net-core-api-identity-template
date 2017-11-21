namespace TemplateProject.Common
{
    public class SearchParameters
    {
        private const int MAX_PAGE_SIZE = 100;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = MAX_PAGE_SIZE;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
            }
        }
        public string SearchQuery { get; set; }
        public string SearchParameter { get; set; }
        public string OrderBy { get; set; }
        public string OrderByDesc { get; set; }
    }
}