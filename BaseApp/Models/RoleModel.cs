using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BaseApp.Constants;

namespace BaseApp.Models
{
    [Table("role")]
    public class RoleModel : BaseModel
    {
        [Column("name")]
        public EnumTypes.RoleType Name { get; set; }

        public Collection<EmpRoleModel> EmpRoleList { get; set; }

    }


}
