using System.ComponentModel.DataAnnotations;

namespace BaseApp.DTO
{
    public class PaginatedResultDTO<T>
    {

        public List<T> ObjectList { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); 

    }
}
