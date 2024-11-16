using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("employee_role")]
    public class EmpRoleModel
    {

        [Column("employee_id")]
        public long EmployeeId {  get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        
        public EmployeeModel Employee { get; set; }

        public RoleModel Role { get; set; }

    }
}
