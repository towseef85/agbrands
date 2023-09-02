using AGBrand.Contracts;
using AGBrand.Implementations;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Contracts.JWT;
using AGBrand.Packages.Filters;
using AGBrand.Packages.Helpers.JWT;
using AGBrand.Packages.Models.Configs.Services;
using AGBrand.Packages.Providers;
using AGBrand.Packages.Services;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AGBrand
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isSecureEnvironment;
        private IServiceCollection _services;
        private IJwtSecurityKey _jwtSecurityKey { get; set; }

        /// <summary>
        /// Startup Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _isSecureEnvironment = bool.Parse(configuration["Settings:IsSecureEnvironment"]);
            ////AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", bool.Parse(configuration["Settings:UseSocketsHttpHandler"]));
        }

        /// <summary>
        /// Configuration Object
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddSingleton(typeof(IConfiguration), _configuration);

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new EncodeActionFilterAttribute());
                //// options.Filters.Add(typeof(ModelValidationAttribute));

                if (!_isSecureEnvironment)
                {
                    options.Filters.Add(typeof(XceptionFilter));
                }

                ////if (cacheProfiles != null)
                ////{
                ////    foreach (var (key, value) in cacheProfiles)
                ////    {
                ////        options.CacheProfiles.Add(key, value);
                ////    }
                ////}
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_configuration["Settings:AppVersion"], new OpenApiInfo
                {
                    Version = _configuration["Settings:AppVersion"],
                    Title = _configuration["Settings:AppName"],
                    Description = _configuration["Settings:AppDescription"],
                    TermsOfService = new Uri(_configuration["Settings:Contacts:Terms"]),
                    Contact = new OpenApiContact
                    {
                        Name = _configuration["Settings:Contacts:DeveloperName"],
                        Email = _configuration["Settings:Contacts:DeveloperEmail"],
                        Url = new Uri(_configuration["Settings:Contacts:Contact"]),
                    },
                    License = new OpenApiLicense
                    {
                        Name = $"Copyright {DateTime.Now.Year}",
                        Url = new Uri(_configuration["Settings:Contacts:License"]),
                    }
                });

                c.OperationFilter<AddHeaderOperationFilter>("x-api-version", "api version", true);
                c.OperationFilter<AddResponseHeadersFilter>(); // [SwaggerResponseHeader]

                ////c.OperationFilter<ExamplesOperationFilter>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = $"Standard Authorization header using the Bearer scheme. Example: \"{JwtBearerDefaults.AuthenticationScheme} XXX\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.UseInlineDefinitionsForEnums();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.OperationFilter<TokenHeaderOperationFilter>();

                ////c.OperationFilter<FileUploadOperationFilter>();

                ////c.SchemaFilter<AutoRestSchemaFilter>();

                ////c.AddEnumsWithValuesFixFilters(services, o =>
                ////{
                ////    // add schema filter to fix enums (add 'x-enumNames' for NSwag) in schema
                ////    o.ApplySchemaFilter = true;

                ////    // add parameter filter to fix enums (add 'x-enumNames' for NSwag) in schema parameters
                ////    o.ApplyParameterFilter = true;

                ////    // add document filter to fix enums displaying in swagger document
                ////    o.ApplyDocumentFilter = true;

                ////    // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' for schema extensions) for applied filters
                ////    o.IncludeDescriptions = true;

                ////    // get descriptions from DescriptionAttribute then from xml-comments
                ////    o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;

                ////    // get descriptions from xml-file comments on the specified path
                ////    // should use "options.IncludeXmlComments(xmlFilePath);" before
                ////    ////o.IncludeXmlCommentsFrom(xmlFilePath);
                ////    // the same for another xml-files...
                ////});
            });

            ////services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            ////var jwtSecurityKey = new JwtAsymmetricSecurityKey(_configuration);

            //////var secret = Configuration["Token:SigningKey"];
            //////var jwtSecurityKey = new JwtSymmetricSecurityKey(secret);

            ////services.AddSingleton(typeof(IJwtSecurityKey), jwtSecurityKey);

            ////services.AddSingleton(typeof(JwtTokenBuilder));

            _jwtSecurityKey = GetJwtCertificateSecurityKey();

            services.AddSingleton(typeof(IJwtSecurityKey), _jwtSecurityKey);

            services.AddScoped(typeof(JwtTokenBuilder));

            services.AddHttpContextAccessor();

            ////services.ConfigureHsts();
            ////services.ConfigureHttpsRedirection();

            services.AddScoped(typeof(IContextLogger), typeof(ContextLogger));

            services.AddScoped(typeof(IWrapper), typeof(Wrapper));

            ////services.AddScoped<IAuthService, Implementations.Services.AuthService>();

            services.AddRepository<SqlContext>(new AddRepositoryConfig
            {
                ConnectionString = _configuration["Settings:SqlServer:DefaultConnection"],
                MigrationAssembly = _configuration["Settings:SqlServer:MigrationAssembly"],
                CommandTimeout = 60,
                RetryOnFailureCount = 2
            });

            ////services.RegisterHttpClients(Configuration);

            ////services.AddApiVersioning(_configuration);

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = !_isSecureEnvironment;
                options.AssumeDefaultVersionWhenUnspecified = !_isSecureEnvironment;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");

                if (_isSecureEnvironment)
                {
                    options.ErrorResponses = new ErrorReponsesSuppressor();
                }
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,

                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Token:ValidIssuer"],

                    ValidateAudience = true,
                    ValidAudience = _configuration["Token:ValidAudience"],
                    IgnoreTrailingSlashWhenValidatingAudience = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _jwtSecurityKey.PublicKey,

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = messageReceivedContext =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = challengeContext =>
                    {
                        challengeContext.HandleResponse();
                        challengeContext.Response.StatusCode = 419;

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = authenticationFailedContext =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = tokenValidatedContext =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            ////services.AddMvc(_configuration, cacheProfiles: GetCacheProfiles());

            ////services.AddSingleton(typeof(IQueueHelper<QueueClient>), new QueueHelper(_configuration));
            ////services.AddSingleton(typeof(IEventHubHelper<EventHubClient>), new EventHubHelper(_configuration));
            ////services.AddSingleton(typeof(INotificationHelper), typeof(NotificationHelper));

            ////services.AddScoped(typeof(IImplementationWrapper<>), typeof(ImplementationWrapper<>));

            ////services.AddResponseCompression(options =>
            ////{
            ////    options.EnableForHttps = true;
            ////    options.Providers.Add<BrotliCompressionProvider>();
            ////    options.Providers.Add<GzipCompressionProvider>();
            ////});

            ////services.Configure<BrotliCompressionProviderOptions>(options =>
            ////{
            ////    options.Level = CompressionLevel.Optimal;
            ////});

            ////services.Configure<GzipCompressionProviderOptions>(options =>
            ////{
            ////    options.Level = CompressionLevel.Fastest;
            ////});

            services.AddHealthChecks()
                .AddAsyncCheck("DataFlowing", async () =>
                {
                    return await Task.FromResult(HealthCheckResult.Healthy()).ConfigureAwait(false);
                });

            ////services.AddResponseCaching();

            ////services.AddPdfConverterService(_configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", config =>
                {
                    config.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                });
            });

            _services = services;
        }

        /// <summary>
        /// Startup Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (!_isSecureEnvironment)
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/{_configuration["Settings:AppVersion"]}/swagger.json", $"{_configuration["Settings:AppName"]} V{_configuration["Settings:AppVersion"]}");

                    c.DisplayRequestDuration();

                    c.DocumentTitle = $"{_configuration["Settings:AppName"]} - {env.EnvironmentName}";

                    ////c.InjectStylesheet(Path.Combine("/swagger/content/theme-update.css"));

                    c.DocExpansion(DocExpansion.None);
                });
            }

            app.Map("/api/auth/jwks",
                builder => builder.Run(context =>
                {
                    var modulus = Base64UrlEncoder.Encode(_jwtSecurityKey.RsaParameters.Modulus);
                    var exponent = Base64UrlEncoder.Encode(_jwtSecurityKey.RsaParameters.Exponent);

                    var jwks = new
                    {
                        keys = new List<dynamic> { new {
                                                            n = modulus,
                                                            e = exponent,
                                                            x5c = new List<string> { _jwtSecurityKey.X5C },
                                                            x5t = _jwtSecurityKey.X5T,
                                                            kid = _configuration["Token:KeyId"],
                                                            alg = SecurityAlgorithms.RsaSha256,
                                                            kty = "RSA",
                                                            use = "sig"
                                                        }
                        }
                    };

                    return context.Response.WriteAsync(JsonConvert.SerializeObject(jwks));
                }));

            ////app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseApiVersioning();

            ////app.UseExceptionHandler("/error");

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });

            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var (message, id) = error.Error.GetExceptionMessage();

                        var logger = loggerFactory.CreateLogger("GlobalException");
                        logger.Log(LogLevel.Critical, message);

                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(new
                        {
                            message = _isSecureEnvironment ? $"Exception Id: {id}" : message
                        }.ToString()).ConfigureAwait(false);
                    }
                });
            });

            ////app.AddSecurityHeaders();

            ////app.UseHsts();

            app.UseHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                            data = e.Value.Data,
                            description = e.Value.Description,
                            duration = e.Value.Duration,
                            exception = e.Value.Exception
                        }),
                        duration = report.TotalDuration
                    });

                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    await context.Response.WriteAsync(result).ConfigureAwait(false);
                }
            });

            ////app.UseResponseCompression();

            ////app.UseResponseCaching();

            ////app.Use(async (context, next) =>
            ////{
            ////    ////context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            ////    ////{
            ////    ////    Public = true,
            ////    ////    MaxAge = TimeSpan.FromSeconds(10)
            ////    ////};

            ////    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new[] { "Accept-Encoding" };

            ////    await next().ConfigureAwait(false);
            ////});

            app.InitMigration(_configuration, (Action<SqlContext, IConfiguration>)SqlContextExtension.Seed);

            app.Use(async (context, next) =>
            {
                ////if (context.Request.Path.Value.Contains(".css"))
                ////{
                ////    context.Response.ContentType = "text/css";

                ////    await context.Response.SendFileAsync(
                ////        env.ContentRootFileProvider.GetFileInfo("/swagger/content/theme-update.css")
                ////    ).ConfigureAwait(false);

                ////    return;
                ////}

                if (!context.Request.Path.Value.Contains("/api"))
                {
                    context.Response.ContentType = "text/html";

                    await context.Response.SendFileAsync(
                        env.ContentRootFileProvider.GetFileInfo("wwwroot/index.html")
                    ).ConfigureAwait(false);

                    return;
                }

                await next().ConfigureAwait(false);
            });

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private JwtCertificateSecurityKey GetJwtCertificateSecurityKey()
        {
            var x509Certificate2 = GetX509Certificate();

            string keyId = _configuration["Token:KeyId"];

            return new JwtCertificateSecurityKey(x509Certificate2, keyId);
        }

        private X509Certificate2 GetX509Certificate()
        {
            var pfxPath = _configuration["Token:PfxPath"];
            var pfxPassword = _configuration["Token:PfxPassword"];
            return new X509Certificate2(pfxPath, pfxPassword);
        }

        ////private static IDictionary<string, CacheProfile> GetCacheProfiles()
        ////{
        ////    IDictionary<string, CacheProfile> cacheProfiles = new ConcurrentDictionary<string, CacheProfile>();

        ////    cacheProfiles.Add("Telemetry",
        ////        new CacheProfile
        ////        {
        ////            Duration = 30,
        ////            VaryByQueryKeys = new[] { "CurrentPage" }
        ////        });

        ////    cacheProfiles.Add("Paged",
        ////        new CacheProfile
        ////        {
        ////            Duration = 30,
        ////            VaryByQueryKeys = new[] { "CurrentPage", "PageSize", "SortDirection", "SortKey", "SearchTerm" }
        ////        });

        ////    cacheProfiles.Add("Never",
        ////        new CacheProfile
        ////        {
        ////            Location = ResponseCacheLocation.None,
        ////            NoStore = true
        ////        });
        ////    return cacheProfiles;
        ////}
    }
}
