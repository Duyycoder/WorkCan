using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("client")]
    public class ClientModel : BaseModel
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("status")]
        public string Status { get; set; }

        public List<ProjectModel> ProjectList { get; set; }

    }
}
