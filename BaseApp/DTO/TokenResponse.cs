namespace BaseApp.DTO
{
    public class TokenResponse
    {

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? AccessTokenExpiryDate { get; set; }

        public DateTime? RefreshTokenExpiryDate { get; set; }

        public long EmpId { get; set; }

        public string FullName { get; set; }    


    }
}
