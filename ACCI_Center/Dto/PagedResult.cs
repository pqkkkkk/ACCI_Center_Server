using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ACCI_Center.Dto
{
    public class PagedResult <T>
    {
        public IEnumerable<T> items { get; set; }
        public int itemCount { get; set; }
        public int currentPageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public PagedResult(IEnumerable<T> items, int itemCount, int currentPageNumber, int pageSize)
        {
            this.items = items;
            this.itemCount = itemCount;
            this.currentPageNumber = currentPageNumber;
            this.pageSize = pageSize;
            this.totalPages = (int)Math.Ceiling((double)itemCount / pageSize);
        }
    }
}
