using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using API.Messages;
using API.Middleware;
using API.Validations;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Managers;
using Application.Mappings;
using Application.ServiceInterfaces;
using Application.Services;
using DAL;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Email;
using Infrastructure.Media;
using Infrastructure.Security;
using Infrastructure.Settings;
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
                opt.UseSqlServer(Configuration.GetValue<string>("DockerConnection") ??
                    Configuration.GetConnectionString("DefaultConnection"));
                opt.EnableSensitiveDataLogging();
            });


            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("WWW-Authenticate")
                    .WithOrigins("http://localhost:3000",
                    "https://ekviti.rs",
                    "http://192.168.0.15:19002",
                    "https://ekvitispa.azurewebsites.net")
                    .AllowCredentials();
                });
            });

            services.AddAutoMapper(typeof(ActivityProfile));

            //Add Scoped Managers
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IUserRecoveryService, UserRecoveryService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IPendingActivityService, PendingActivityService>();
            services.AddScoped<IDiceService, DiceService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IHappeningService, HappeningService>();
            services.AddScoped<IChallengeService, ChallengeService>();

            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IPhotoAccessor, CloudinaryPhotoAccessor>();
            services.AddScoped<IFacebookAccessor, FacebookAccessor>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IActivityCounterManager, ActivityCounterManager>();
            services.AddScoped<IEmailManager, EmailManager>();


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
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
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
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddSwaggerGen(c =>

            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetEntryAssembly().GetName().Name, Version = "v1" });
            });

            services.AddApplicationInsightsTelemetry();
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
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}

