using IdentityServer4Test.IDP.Models;
using IdentityServer4Test.IDP.Data;
using IdentityServer4Test.IDP.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Test.IDP
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }
        private string connectionString;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = HostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddTransient<SeedConfiguration>();
            AddEntityFramework(services);
            AddIdentityServer(services);
            AddIdentityAuthentication(services);
           // services.AddAutoMapper(typeof(Startup));
            services.AddMvc();
            AddAuthorization(services);
        }

        private void AddEntityFramework(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityCore<ApplicationUser>(x =>
            {
                x.Password.RequireDigit = true;
                x.Password.RequireLowercase = true;
                x.Password.RequireUppercase = true;
                x.Password.RequireNonAlphanumeric = true;
                x.Password.RequiredLength = 8;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }

        private void AddIdentityAuthentication(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters();
        }

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
           {
               options.AddPolicy("GetValues", policy => policy.RequireClaim(ClaimTypes.Role, "superAdmin"));
               options.AddPolicy("PutValues", policy => policy.RequireClaim("test_claim", "3333"));
               options.AddPolicy("GetAllValues", policy => policy.RequireClaim("test_claim", "3333"));
               options.AddPolicy("IsAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "superAdmin"));
           });
        }

        public void AddIdentityServer(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = services.AddIdentityServer()
              .AddAspNetIdentity<ApplicationUser>()
              .AddProfileService<ProfileService>()
               .AddDeveloperSigningCredential()
              .AddConfigurationStore(options =>
              {
                  options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
              });


            //services.AddIdentityServer(options =>
            //{
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseSuccessEvents = true;
            //})
            //   .AddDeveloperSigningCredential()
            //   .AddAspNetIdentity<ApplicationUser>()
            //   .AddProfileService<ProfileService>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedConfiguration seedConfig)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();
            seedConfig.SeedData().Wait();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
