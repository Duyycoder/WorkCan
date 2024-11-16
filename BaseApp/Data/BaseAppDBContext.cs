using Microsoft.EntityFrameworkCore;
using BaseApp.Models;
using static BaseApp.Constants.EnumTypes;

namespace BaseApp.Data
{
    public class BaseAppDBContext : DbContext
    {

        private readonly IHttpContextAccessor _contextAccessor;

        public BaseAppDBContext(DbContextOptions<BaseAppDBContext> dbContextOptions, IHttpContextAccessor httpContextAccessor)
            : base(dbContextOptions)
        {
            _contextAccessor = httpContextAccessor;
        }

        public DbSet<ActivityModel> ActivityList { get; set; }

        public DbSet<ApplicationModel> ApplicationList { get; set; }

        public DbSet<CompanyInfoModel> CompanyInfoList { get; set; }

        public DbSet<EventModel> EventList { get; set; }

        public DbSet<LocationModel> LocationList { get; set; }

        public DbSet<NotificationModel> NotificationList { get; set; }

        public DbSet<EmployeeModel> EmployeeList { get; set; } = default!;

        public DbSet<ClientModel> ClientList { get; set; } = default!;

        public DbSet<EmpRoleModel> EmpRoleList { get; set; } = default!;

        public DbSet<GiftModel> GiftList { get; set; } = default!;

        public DbSet<GiftRequestModel> GiftRequestList { get; set; } = default!;

        public DbSet<PointModel> PointList { get; set; } = default!;

        public DbSet<ProjectModel> ProjectList { get; set; } = default!;
         
        public DbSet<RoleModel> RoleList { get; set; } = default!;

        public DbSet<TaskModel> TaskList { get; set; } = default!;

        public DbSet<DeviceModel> DeviceList { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<EmpRoleModel>()
                .HasKey(value => new { value.EmployeeId, value.RoleId });

            modelBuilder.Entity<GiftRequestModel>()
                .HasKey(value => new { value.EmpId, value.GiftId });

            // change enum to string
            modelBuilder.Entity<RoleModel>()
                .Property(role => role.Name)
                .HasConversion<string>();

            modelBuilder.Entity<ProjectModel>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationModel>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationModel>()
                .Property(p => p.Type)
                .HasConversion<string>();

            modelBuilder.Entity<ActivityModel>()
                .Property(a => a.Type)
                .HasConversion<string>();

           modelBuilder.Entity<GiftRequestModel>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskModel>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<EmployeeModel>()
                .HasMany(emp => emp.GiftRequestList)
                .WithOne(empGift => empGift.Employee)
                .HasForeignKey(empGift => empGift.EmpId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeModel>()
               .HasMany(emp => emp.LocationList)
               .WithOne(location => location.Employee)
               .HasForeignKey(location => location.EmpId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeModel>()
                .HasIndex(emp => emp.Code)
                .IsUnique(true);

            modelBuilder.Entity<EmployeeModel>()
                .HasMany(emp => emp.EmpRoleList)
                .WithOne(empRole => empRole.Employee)
                .HasForeignKey(empRole => empRole.EmployeeId);

            modelBuilder.Entity<RoleModel>()
                .HasMany(role => role.EmpRoleList)
                .WithOne(empRole => empRole.Role)
                .HasForeignKey(empRole => empRole.RoleId);

            modelBuilder.Entity<RoleModel>()
            .Property(e => e.Name)
            .HasConversion(
                v => v.ToString(),
                v => (RoleType)Enum.Parse(typeof(RoleType), v)
            );

            modelBuilder.Entity<EmployeeModel>()
                .HasMany(emp => emp.DeviceList)
                .WithOne(device => device.Employee)
                .HasForeignKey(device => device.EmpId);

            modelBuilder.Entity<ActivityModel>()
                .HasOne(a => a.ParentActivity)
                .WithMany(c => c.ChildActivities)
                .HasForeignKey(c => c.ParentActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationModel>()
                .Property(a => a.Note)
                .HasColumnType("nvarchar(max)");

            // init admin account
            modelBuilder.Entity<EmployeeModel>().HasData(new EmployeeModel
            {
                Id = 1,
                Code = "Code1",
                FullName = "superadmin",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Email = "superadmin@gmail.com",
                PhoneNum = "+84388124368",
                Avatar = "https://res.cloudinary.com/duylinhmedia/image/upload/v1725848701/rje2a5ibauortais1xrl.jpg",
                Address = "Ha Noi",
                CreatedDate = DateTime.Now,
                CreatedBy = 1.ToString(),
                UpdatedDate = DateTime.Now,
                UpdatedBy = 1.ToString()
            });


            // init new role record
            modelBuilder.Entity<RoleModel>().HasData(new RoleModel
            {
                Id = 1,
                Name = RoleType.ROLE_ADMIN,
                ActiveFlag = true,
                DeleteFlag = false,
                CreatedDate = DateTime.Now,
                CreatedBy = 1.ToString(),
                UpdatedDate = DateTime.Now,
                UpdatedBy = 1.ToString()
            });

            modelBuilder.Entity<RoleModel>().HasData(new RoleModel
            {
                Id = 2,
                Name = RoleType.ROLE_EMPLOYEE,
                ActiveFlag = true,
                DeleteFlag = false,
                CreatedDate = DateTime.Now,
                CreatedBy = 1.ToString(),
                UpdatedDate = DateTime.Now,
                UpdatedBy = 1.ToString()
            });

            // set role for new employee record
            modelBuilder.Entity<EmpRoleModel>().HasData(new EmpRoleModel
            {
                EmployeeId = 1,
                RoleId = 1,
            });

            modelBuilder.Entity<EmpRoleModel>().HasData(new EmpRoleModel
            {
                EmployeeId = 1,
                RoleId = 2,
            });

            base.OnModelCreating(modelBuilder);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            // entries is a list of entities that's tracked by changtracker
            // they could be added or modified
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseModel &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            // get current user's information from current http request
            // if user cannot be found, it would be "system" added to createdBy and updatedBy
            var currentUser = _contextAccessor.HttpContext?.User?.FindFirst("Id")?.Value ?? "System";

            // update entries with information from currentUser
            foreach (var entityEntry in entries)
            {
                var baseModel = (BaseModel) entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    baseModel.CreatedDate = DateTime.Now;
                    baseModel.CreatedBy = currentUser;
                }

                baseModel.UpdatedDate = DateTime.Now;
                baseModel.UpdatedBy = currentUser;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
