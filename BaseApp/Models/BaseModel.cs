using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BaseApp.Models
{
    public abstract class BaseModel
    {

        [Column("id"), Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("active_flag"), DefaultValue(true)]
        public bool ActiveFlag { get; set; } = true;

        [Column("delete_flag"), DefaultValue(false)]
        public bool DeleteFlag { get; set; } = false;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("created_by")]
        public string CreatedBy { get; set; } = string.Empty;

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [Column("updated_by")]
        public string UpdatedBy { get; set; } = string.Empty;

    }
}
