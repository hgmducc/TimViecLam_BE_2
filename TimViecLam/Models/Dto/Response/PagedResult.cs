namespace TimViecLam.Models.Dto.Response
{
    public class PagedResult<T>
    {
        public bool IsSuccess { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
        public List<T> Data { get; set; } = new List<T>();

        // Pagination info
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}