using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4Test.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddMVCCoreAndPolicies(services);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.Audience = "IdentityServer4Test.Api";
                });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthorization(options =>  {
                options.AddPolicy("DeleteValues", policy => policy.RequireClaim(ClaimTypes.Role, "superAdmin"));
                options.AddPolicy("GetValues", policy => policy.RequireClaim("test_claim", "3333"));
                options.AddPolicy("GetAllValues", policy => policy.RequireClaim("test_claim", "3333"));
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "superAdmin"));
            });
        }

        public void AddMVCCoreAndPolicies(IServiceCollection services)
        {
            services.AddMvcCore()
            .AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
