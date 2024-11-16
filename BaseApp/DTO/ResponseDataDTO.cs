namespace BaseApp.DTO
{
    public class ResponseDataDTO<DataType>
    {
         public DataType Data { get; set; } = default;

         public bool Succeed { get; set; } = true;

         public int Code { get; set; }

        public string ErrorMessage { get; set; }
    }
}
