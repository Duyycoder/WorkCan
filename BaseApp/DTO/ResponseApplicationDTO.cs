using BaseApp.Constants;
using BaseApp.Models;

namespace BaseApp.DTO
{
    public class ResponseApplicationDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<SingleResponseApplicationDTO> ApplicationList { get; set; }

        public class SingleResponseApplicationDTO 
        {

            public long Id { get; set; }    

            public string Title { get; set; }

            public string StartedDate { get; set; }

            public string EndedDate { get; set; }

            public string Reason { get; set; }

            public EnumTypes.ApplicationType Type { get; set; }

            public EnumTypes.ApplicationStatus Status { get; set; }

            public string CreatedDate { get; set; } 

        }

    }
}