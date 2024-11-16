using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BaseApp.Models;

[Table("employee")]
public class EmployeeModel : BaseModel
{
    [Column("code")]
    public string? Code { get; set; }    

    [Column("full_name")]
    public string FullName { get; set; }

    [Column("github_name")]
    public string? GithubName { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("mail")]
    public string Email { get; set; }

    [Column("phone_num")]
    public string PhoneNum { get; set; }

    [Column("avatar")]
    public string? Avatar { get; set; }

    [Column("address")]
    public string Address { get; set; }

    [NotMapped]
    public string? AccessToken { get; set; }

    [Column("refresh_token"), AllowNull]
    public string? RefreshToken { get; set; }

    [Column("rt_expiry_time"), AllowNull]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<GiftRequestModel> GiftRequestList { get; set; }

    public List<EmpRoleModel> EmpRoleList { get; set; }

    public ICollection<LocationModel> LocationList { get; set; }

    public ICollection<DeviceModel> DeviceList { get; set; }

}
