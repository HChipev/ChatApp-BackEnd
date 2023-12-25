using System.Reflection;
using System.Text;
using Data;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Admin.Profiles;
using Data.ViewModels.RabbitMQ.Models;
using Infrastructure.Filters;
using Infrastructure.Handlers;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Abstract;
using Service.EventHandlers.Implementations;
using Service.EventHandlers.Interfaces;
using Service.Implementations;
using Service.Interfaces;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterDbContext(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Tokens.EmailConfirmationTokenProvider = "Default";
                });
            }

            if (environment.IsProduction())
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(Environment.GetEnvironmentVariable("MSSQL_URL")));

                using var context = new DataContext(new DbContextOptionsBuilder<DataContext>()
                    .UseSqlServer(Environment.GetEnvironmentVariable("MSSQL_URL")).Options);
                context.Database.Migrate();

                services.Configure<IdentityOptions>(options => { options.SignIn.RequireConfirmedEmail = false; });
            }

            services.AddIdentity<User, Role>().AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddHttpClient();

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScopedServiceTypes(typeof(TokenService).Assembly, typeof(IService));

            if (environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.WithOrigins("http://localhost:5173","https://bytebuddy.app")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithExposedHeaders("*");
                    });
                });
            }
            else if (environment.IsProduction())
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.WithOrigins("https://bytebuddy.app", "https://www.bytebuddy.app",
                                "http://localhost:5173/")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithExposedHeaders("*");
                    });
                });
            }

            return services;
        }

        public static IServiceCollection RegisterRabbitMqBus(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IEventBus, RabbitMqEventBus>(sp =>
            {
                var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST_NAME") ??
                               configuration["RabbitMQ:HostName"];
                var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER_NAME") ??
                               configuration["RabbitMQ:UserName"];
                var password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ??
                               configuration["RabbitMQ:Password"];
                var virtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VIRTUAL_HOST") ??
                                  configuration["RabbitMQ:VirtualHost"];

                var connectionString = $"amqps://{userName}:{password}@{hostName}/{virtualHost}";

                var rabbitMq = new RabbitMqEventBus(connectionString, sp.GetRequiredService<IServiceScopeFactory>());
                rabbitMq.Subscribe<GenerateAnswerQueue, GenerateAnswerEventHandler>();
                rabbitMq.Subscribe<SaveDocumentsQueue, SaveDocumentsEventHandler>();

                return rabbitMq;
            });

            services.AddScoped<IEventHandler<GenerateAnswerQueue>, GenerateAnswerEventHandler>();
            services.AddScoped<IEventHandler<SaveDocumentsQueue>, SaveDocumentsEventHandler>();

            return services;
        }

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(UserProfile));

            return services;
        }

        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ??
                                                configuration["JWT:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portfolio-BackEnd", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Please insert JWT token into field"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static IServiceCollection RegisterFilters(this IServiceCollection services)
        {
            services.AddControllers(options => { options.Filters.Add<CustomExceptionFilter>(); });

            return services;
        }

        public static IServiceCollection RegisterSignalR(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }

        private static IServiceCollection AddScopedServiceTypes(this IServiceCollection services, Assembly assembly,
            Type fromType)
        {
            var serviceTypes = assembly.GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.IsClass && !x.IsAbstract &&
                            fromType.IsAssignableFrom(x))
                .Select(x => new
                {
                    Interface = x.GetInterface($"I{x.Name}"),
                    Implementation = x
                });
            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType.Interface, serviceType.Implementation);
            }

            return services;
        }
    }
}