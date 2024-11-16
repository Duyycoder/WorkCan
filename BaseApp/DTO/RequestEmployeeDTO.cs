using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseApp.DTO
{
    public class RequestEmployeeDTO
    {

        public string FullName { get; set; }

        public string GithubName { get; set; }  

        public string? Password { get; set; }

        public string Email { get; set; }  

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public IFormFile? Avatar { get; set; }

    }
}
