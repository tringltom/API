using Application.Managers;
using Application.Repositories;
using API.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using Application.Services;
using API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Application.Security;

namespace API
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
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAutoMapper(typeof(UserProfile));

            //Add Transient Repositories
            services.AddTransient<IValueRepository, ValueRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            //Add Scoped Managers
            services.AddScoped<IValueManager, ValueManager>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<DataContext>();


            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    opt.Events = new JwtBearerEvents
                    {
                        //OnMessageReceived = context =>
                        //{
                        //    var accessToken = context.Request.Query["access_token"];
                        //    var path = context.HttpContext.Request.Path;
                        //    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                        //    {
                        //        context.Token = accessToken;
                        //    }

                        //    return Task.CompletedTask;
                        //}
                    };
                });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
