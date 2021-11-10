using System;
using System.Reflection;
using System.Text;
using API.Mappings;
using API.Middleware;
using API.Validations;
using Application.Managers;
using Application.Media;
using Application.Repositories;
using Application.Security;
using Application.ServiceHelpers;
using Application.ServiceInterfaces;
using Application.Services;
using Application.Settings;
using Domain.Entities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;

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
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                opt.EnableSensitiveDataLogging();
            });


            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("WWW-Authenticate").WithOrigins("http://localhost:3000", "https://ekviti.rs").AllowCredentials();
                });
            });

            services.AddAutoMapper(typeof(ActivityProfile));

            services.AddSingleton<IUserServiceHelper, UserServiceHelper>();

            //Add Transient Repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IActivityRepository, ActivityRepository>();

            //Add Scoped Managers
            services.AddScoped<IUserManager, UserManager>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserRecoveryService, UserRecoveryService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IPhotoAccessor, CloudinaryPhotoAccessor>();
            services.AddScoped<IFacebookAccessor, FacebookAccessor>();

            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DataContext>();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));


            services.Configure<IdentityOptions>(options =>
            {
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
                });

            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation(cfg =>
            {
                cfg.RegisterValidatorsFromAssemblyContaining<UserEmailVerificationValidation>();
            }).AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSwaggerGen(c =>

            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetEntryAssembly().GetName().Name, Version = "v1" });
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>

            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetEntryAssembly().GetName().Name);
            });

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

