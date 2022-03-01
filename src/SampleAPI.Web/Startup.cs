namespace SampleAPI.Web;

using Kkokkino.SampleAPI.Persistence;
using Kkokkino.SampleAPI.Persistence.Identity;
using Kkokkino.SampleAPI.Web.Service;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IO;
using System.Reflection;
using System.Text;

public class Startup
{
  public Startup(IConfiguration configuration, IWebHostEnvironment environment)
  {
    Configuration = configuration;
    Environment = environment;
  }

  public IConfiguration Configuration { get; }

  public IWebHostEnvironment Environment { get; }

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddDbContext<DataProtectionDbContext>(options => options
      .UseNpgsql(Configuration.GetConnectionString("DataProtectionDb")));

    services.AddDbContext<PersistenceContext>(options => options
      .UseNpgsql(Configuration.GetConnectionString("SampleApiDb"), pgsql => pgsql
        .EnableRetryOnFailure(3)
        .SetPostgresVersion(13, 4))
      .EnableDetailedErrors(Environment.IsDevelopment())
      .EnableSensitiveDataLogging(Environment.IsDevelopment())
      .ConfigureWarnings(warn => warn
        .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
        .Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
        .Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan)));

    services.AddDataProtection()
      .SetApplicationName($"{Environment.ApplicationName}-{Environment.EnvironmentName}")
      .PersistKeysToDbContext<DataProtectionDbContext>();

    services.AddHostedService<BatchEmailService>();

    services.AddIdentity<SampleApiUser, SampleApiRole>(options =>
      {
        var isDevelopment = Environment.IsDevelopment();
        options.SignIn.RequireConfirmedAccount = !isDevelopment;
        options.SignIn.RequireConfirmedEmail = !isDevelopment;
        options.SignIn.RequireConfirmedPhoneNumber = false;

        options.User.RequireUniqueEmail = !isDevelopment;

        options.Password.RequireDigit = !isDevelopment;
        options.Password.RequireLowercase = !isDevelopment;
        options.Password.RequireNonAlphanumeric = !isDevelopment;
        options.Password.RequireUppercase = !isDevelopment;
      })
      .AddEntityFrameworkStores<PersistenceContext>()
      .AddDefaultTokenProviders();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => // Auth service is JWT
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true, // arxi ekdosis
          ValidateAudience = true, // an ekdidw gia polla apps p.x. na einai valid mono gia ena
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = Configuration["JWT:Issuer"], // TODO sta app settings
          ValidAudience = Configuration["JWT:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
        };
      });

    services.AddSwaggerGen(c =>
    {
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
    });
    services.AddMvc();
    services.AddRazorPages();
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  // Thelous sygkekrimeni seira, einai layers.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
      x.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleAPI");
    });

    app.UseRouting();

    app.UseAuthentication(); // poios eisai

    app.UseAuthorization(); // ti mporeis na kaneis

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapRazorPages();
    });
  }
}
