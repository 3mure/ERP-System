using BuildingBlocks.Interfaces;
using BuildingBlocks.SharedEntities;
using Catalog_Service.Features.Alerts.Catalog_Service.BackgroundJobs;
using Catalog_Service.Infrastructure;
using Catalog_Service.Infrastructure.Data;
using Catalog_Service.Infrastructure.UnitOfWork;
using ERP_Service.Grpc;
using Catalog_Service.Messaging;
using Catalog_Service.Messaging.Consumers;
using Catalog_Service.Messaging.Inventory;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
// PromotionGrpcService should be in Catalog_Service.GrpcServices namespace
namespace Catalog_Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // -------------------------------------------------------------------------------------
            // 1. Serilog Configuration
            // -------------------------------------------------------------------------------------
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "CatalogService")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
                .WriteTo.File("logs/CatalogService-.log", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting CatalogService Application");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                var config = builder.Configuration;

                // -------------------------------------------------------------------------------------
                // 2. Service Registration (Dependency Injection)
                // -------------------------------------------------------------------------------------

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowReactApp",
                        policy =>
                        {
                            policy.WithOrigins(
                                        "http://localhost:5173", // ??? ??? Development
                                        "https://erb-management-dashboard.vercel.app" // ??? ??? Production
                                   )
                                   .AllowAnyHeader()
                                   .AllowAnyMethod();
                        });
                });

                // ... ??? builder.Build() ...

                

                builder.Services.AddMemoryCache();
                builder.Services.AddHttpContextAccessor();
                // In your CatalogService's Program.cs or Startup.cs
                // Register all repositories from BuildingBlocks
                





                // Ensure you have these classes created in your project or referencing BuildingBlocks
                // builder.Services.AddScoped<ICurrentUserService, CurrentUserService>(); 
                // builder.Services.AddScoped<TransactionMiddleware>(); 
                 builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddHostedService<AlertsWorker>();

                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    var redisUrl = builder.Configuration["Redis:Url"];
                    var redisPassword = builder.Configuration["Redis:Password"];

                    if (string.IsNullOrEmpty(redisUrl))
                        throw new ArgumentException("Redis URL is missing!");

                    options.Configuration = $"{redisUrl},password={redisPassword}";
                    options.InstanceName = "catalog_"; // prefix ?????
                });

                // Configure Entity Framework Core with SQL Server
                builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

                    // Recommended for Read-Heavy services like Catalog
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                    if (builder.Environment.IsDevelopment())
                    {
                        options.EnableSensitiveDataLogging(true);
                        options.EnableDetailedErrors(true);
                    }
                });
        


                // Generic Repository Registration
                var entityTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)))
                    .ToList();

                foreach (var entityType in entityTypes)
                {
                    var interfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);
                    var implementationType = typeof(BaseRepository<>).MakeGenericType(entityType);

                    builder.Services.AddScoped(interfaceType, implementationType);
                }
                builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                Log.Information("Registered {Count} generic repositories successfully", entityTypes.Count);

                builder.Services.AddMediatR(typeof(Program).Assembly);

                builder.Services.Configure<RabbitMqOptions>(config.GetSection(RabbitMqOptions.SectionName));
                builder.Services.AddScoped<IInventoryStockService, InventoryStockService>();

                // -------------------------------------------------------------------------------------
                // MassTransit — listen to RabbitMQ and run consumers
                // -------------------------------------------------------------------------------------
                builder.Services.AddMassTransit(x =>
                {
                    x.AddConsumer<ReserveStockConsumer>();
                    x.AddConsumer<CommitStockConsumer>();
                    x.AddConsumer<PaymentSucceededStockConsumer>();
                    x.AddConsumer<OrderCreatedSalesOrderConsumer>();
                    x.AddConsumer<Catalog_Service.Features.ProductsFeature.StockManagement.OrderDeliveredConsumer>();
                    x.AddConsumer<Catalog_Service.Features.ProductsFeature.StockManagement.PaymentFailedConsumer>();
                    x.AddConsumer<Catalog_Service.Features.ProductsFeature.StockManagement.OrderCancelledConsumer>();
                    x.AddConsumer<Catalog_Service.Features.ProductsFeature.OfferExpiredConsumer>();

                    x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                    {
                        o.UseSqlServer();
                        o.UseBusOutbox();
                    });

                    x.SetKebabCaseEndpointNameFormatter();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var rabbit = context.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;

                        cfg.Host(rabbit.Host, rabbit.VirtualHost, h =>
                        {
                            h.Username(rabbit.Username);
                            h.Password(rabbit.Password);
                        });

                        // Listen to an existing (pre-created) exchange/queue if configured.
                        // This is intentionally explicit so we don't rely on MassTransit's auto-topology.
                        if (!string.IsNullOrWhiteSpace(rabbit.OrderCreatedQueue))
                        {
                            cfg.ReceiveEndpoint(rabbit.OrderCreatedQueue, e =>
                            {
                                e.ConfigureConsumeTopology = false;

                                if (!string.IsNullOrWhiteSpace(rabbit.OrderCreatedExchange))
                                {
                                    e.Bind(rabbit.OrderCreatedExchange, b =>
                                    {
                                        b.ExchangeType = string.IsNullOrWhiteSpace(rabbit.OrderCreatedExchangeType)
                                            ? "topic"
                                            : rabbit.OrderCreatedExchangeType;

                                        if (!string.IsNullOrWhiteSpace(rabbit.OrderCreatedRoutingKey))
                                            b.RoutingKey = rabbit.OrderCreatedRoutingKey;
                                    });
                                }

                                e.ConfigureConsumer<OrderCreatedSalesOrderConsumer>(context);
                            });
                        }

                        cfg.ConfigureEndpoints(context);
                    });
                });

                builder.Services.AddOptions<MassTransitHostOptions>()
                    .Configure(o => o.WaitUntilStarted = true);

                // -------------------------------------------------------------------------------------
                // API Security & Configuration
                // -------------------------------------------------------------------------------------

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        b => b.AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials());
                });

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                    };
                });

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                });

                // Add gRPC service
                builder.Services.AddGrpc();
                builder.Services.AddGrpc(options =>
                {
                    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
                    options.MaxReceiveMessageSize = 16 * 1024 * 1024;  // 16 MB
                    options.MaxSendMessageSize = 16 * 1024 * 1024;
                });
             

                // Add gRPC client for Ordering Service (US-A11: Check product in active orders)
                builder.Services.AddGrpcClient<BuildingBlocks.Grpc.OrderingGrpc.OrderingGrpcClient>(options =>
                {
                    var orderingServiceUrl = config["GrpcServices:OrderingService"] ?? "https://localhost:5199";
                    options.Address = new Uri(orderingServiceUrl);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    // Allow self-signed certificates in development
                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    return handler;
                });

                builder.Services.AddEndpointsApiExplorer();
                // Use controllers instead of minimal API endpoint mapping
                builder.Services.AddControllers();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogService API", Version = "v1" });

                    // Avoid schemaId collisions when different types share the same short name (e.g. ProductSpecificationDto in multiple features).
                    options.CustomSchemaIds(type => type.FullName!.Replace('+', '.'));

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter JWT with Bearer into field (e.g., 'Bearer {token}')",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new string[] {}
                        }
                    });
                });

                var app = builder.Build();
                app.UseCors("AllowReactApp");

                // -------------------------------------------------------------------------------------
                // 3. Database Migration & Seeding (Startup Scope)
                // -------------------------------------------------------------------------------------
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        //await context.Database.MigrateAsync();
                        //await DatabaseSeeder.SeedAsync(services);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred during database migration or seeding");
                        if (app.Environment.IsDevelopment()) throw;
                    }
                }

                // -------------------------------------------------------------------------------------
                // 4. HTTP Request Pipeline (Middleware Order)
                // -------------------------------------------------------------------------------------

                // Uncomment once you have the Middleware class
                // app.UseMiddleware<ErrorHandlingMiddleware>();

                // Swagger in all environments so ERP APIs are visible (restrict via reverse proxy / auth in production if needed).
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // Relative to the Swagger UI route prefix (/swagger) so it works behind path bases and proxies.
                    options.SwaggerEndpoint("v1/swagger.json", "ERP / Catalog API v1");
                    options.DocumentTitle = "ERP / Catalog API";
                });

                ////app.UseHttpsRedirection();

                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();
                var publicBaseUrl = (config["ErpGrpc:PublicBaseUrl"] ?? "https://management.runasp.net").TrimEnd('/');

                app.MapGet("/", () => Results.Ok(new
                {
                    service = "Catalog / ERP",
                    status = "running",
                    integration = $"{publicBaseUrl}/grpc/info"
                }));

                app.MapGet("/health", () => Results.Ok("healthy"));

                app.MapGet("/grpc/info", () => Results.Ok(new
                {
                    contractVersion = "1.0",
                    serverUrl = publicBaseUrl,
                    protocol = "grpc",
                    transport = "HTTP/2 over HTTPS",
                    clientSdk = "Erp.Grpc.Client (reference from ERP_System solution)",
                    configurationSection = "ErpGrpc:ServerUrl",
                    services = new[]
                    {
                        new { name = "CatalogGrpc", package = "catalog", rpcs = new[] { "GetProduct", "GetProductsByIds", "GetBranchProducts", "ValidateStock" } },
                        new { name = "PromotionGrpc", package = "promotion", rpcs = new[] { "ValidateCoupon", "RedeemPoints", "GetAdjustedPrices" } }
                    },
                    protoFiles = new[] { "catalog.proto", "promotion.proto" },
                    rest = new { swagger = $"{publicBaseUrl}/swagger" }
                }));

                // Map controller routes (replace minimal API MapAllEndpoints)
                app.MapControllers();

                // Map gRPC services (HTTP/2 — same host as REST when deployed on IIS/Kestrel)
                app.MapGrpcService<CatalogGrpcService>();
                app.MapGrpcService<PromotionGrpcService>();


                // Uncomment once you have the Middleware class
                // app.UseMiddleware<TransactionMiddleware>();



                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start due to an unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}