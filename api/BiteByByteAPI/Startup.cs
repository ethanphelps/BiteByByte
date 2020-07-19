using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using AutoMapper;
using BiteByByteAPI.Models;
using BiteByByteAPI.Services;
using BiteByByteAPI.Helpers;
using BiteByByteAPI.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using WebApi.Helpers;

namespace BiteByByteAPI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            services.Configure<BiteByByteDatabaseSettings>(
                _configuration.GetSection(nameof(BiteByByteDatabaseSettings)));
            
            services.AddSingleton<IBiteByByteDatabaseSettings>(sp => 
                sp.GetRequiredService<IOptions<BiteByByteDatabaseSettings>>().Value);
            
            services.AddSingleton<UserService>();
            
            
            // configure strongly typed settings objects
            // _configuration.GetSection("AppSettings") pulls from secrets.json!!
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var userId = /*int.Parse*/(context.Principal.Identity.Name);
                            var user = userService.GetById(userId/*.ToString()*/);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            
            // enable CORS for angular application
            // app.UseCors(builder =>
            //     builder.WithOrigins("http://localhost:4200"));
            
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
        }
    }
}