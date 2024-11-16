using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("gift")]
    public class GiftModel : BaseModel
    {

        [Column("image")]
        public string Image { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("quantity")]
        public int Quantity {  get; set; }

    }
}
