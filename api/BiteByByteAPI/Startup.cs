using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiteByByteAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using BiteByByteAPI.Models;
using BiteByByteAPI.Services;

namespace BiteByByteAPI
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
            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());;

            services.Configure<BiteByByteDatabaseSettings>(
                Configuration.GetSection(nameof(BiteByByteDatabaseSettings)));
            
            services.AddSingleton<IBiteByByteDatabaseSettings>(sp => 
                sp.GetRequiredService<IOptions<BiteByByteDatabaseSettings>>().Value);
            
            services.AddSingleton<UserService>();
            
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // enable CORS for angular application
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:4200"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}