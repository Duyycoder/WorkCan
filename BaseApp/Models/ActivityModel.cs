using System.ComponentModel.DataAnnotations.Schema;
using BaseApp.Constants;

namespace BaseApp.Models
{
    [Table("activity")]
    public class ActivityModel : BaseModel
    {
        [Column("type")]
        public EnumTypes.ActivityType Type { get; set; }

        [Column("activity_time")]
        public TimeOnly ActivityTime { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("location_id"), ForeignKey(nameof(LocationModel.Id))]
        public long LocationId { get; set; }

        public LocationModel Location { get; set; }

        [Column("parent_activity_id")]
        public long? ParentActivityId { get; set; }

        public ActivityModel ParentActivity { get; set; }

        public ICollection<ActivityModel> ChildActivities { get; set; } = new List<ActivityModel>();
    }
}
