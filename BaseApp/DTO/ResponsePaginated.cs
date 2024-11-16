namespace BaseApp.DTO
{
    public class ResponsePaginated<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }   

        public int TotalRecords { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public List<T> Items { get; set; }

        public ResponsePaginated(List<T> items, int pageNumber, int pageSize, int totalRecords)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }

    }
}
