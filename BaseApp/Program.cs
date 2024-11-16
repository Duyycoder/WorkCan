using Microsoft.EntityFrameworkCore;
using BaseApp.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using BaseApp;
using BaseApp.Models;
using BaseApp.Service;
using Serilog;
using BaseApp.Repository;
using BaseApp.DTO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using CloudinaryDotNet;
using BaseApp.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BaseApp.Repository.Base;
using Microsoft.AspNetCore.HttpOverrides;
// write Log in cmd
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary").Get<CloudinaryConfig>();

    var account = new Account(
        cloudinaryConfig.CloudName,
        cloudinaryConfig.ApiKey,
        cloudinaryConfig.ApiSecret
    );

    var cloudinary = new Cloudinary(account);
    builder.Services.AddSingleton(cloudinary);

    // Add services to the container.

    // JWT
    builder.Services.AddAuthentication(opt => {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricKeys:MySymmetricKey"])),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Roles", "ROLE_ADMIN"));
        options.AddPolicy("EmployeePolicy", policy => policy.RequireClaim("Roles", "ROLE_EMPLOYEE"));
    });

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console());

    // Add FluentValidation
    builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RequestEmployeeDTO>());

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(
            "v1", new OpenApiInfo { 
                Title = "WorkScan API Documentation", Version = "v1" ,
                Description = "All-in-one API document for WorkScan Project"
            });

        c.MapType<ResponseDataDTO<object>>(() => new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
        {
            { "data", new OpenApiSchema { Type = "string", Example = new OpenApiString("Example data") } },
            { "succeed", new OpenApiSchema { Type = "boolean", Example = new OpenApiBoolean(true) } },
            { "errorCode", new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(0) } },
            { "errorMessage", new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Example = new OpenApiString("Example error") } } }
        }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter 'Bearer' followed by a space and your JWT token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        // Require JWT authentication for all operations
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

    });

    builder.Services.AddDbContext<BaseAppDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BaseAppDBContext")
            ?? throw new InvalidOperationException("Connection string 'BaseAppDBContext' not found.")));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
    });

    builder.Services.AddScoped<IRepositoryManager, RepositoryManager>(); 

    builder.Services.AddScoped<IEmployeeService, EmployeeService>();

    builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

    builder.Services.AddScoped<ITokenService, TokenSerivce>();

    builder.Services.AddScoped<ICompanyInfoService, CompanyInfoService>();

    builder.Services.AddScoped<ILocationService, LocationService>();    

    builder.Services.AddScoped<IActivityService, ActivityService>();

    builder.Services.AddScoped<IDeviceService, DeviceService>();

    builder.Services.AddScoped<IApplicationService, ApplicationService>();

    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => 
        {
            c.DefaultModelExpandDepth(2);
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.DisplayRequestDuration();
        });
    }

    app.UseCors("AllowSpecificOrigin");

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated");
}
finally
{
    Log.CloseAndFlush();
}