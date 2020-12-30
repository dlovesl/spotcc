using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestEase;
using Spotcc;
using Spotcc.Jobs;
using Spotcc.Services;
using Spotcc.Services.Models;
using SpotCC.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using VueCliMiddleware;

namespace SpotCC
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
            services.AddHealthChecks()
                .AddCheck("hc", () => HealthCheckResult.Healthy());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var hangfireConnectionString = Configuration.GetConnectionString("Hangfire");

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseColouredConsoleLogProvider()
                .UseSQLiteStorage(hangfireConnectionString));
            
            services.AddHangfireServer();

            var channel = Channel.CreateBounded<PlayCount>(1);

            services.AddSingleton(channel);
            services.AddScoped(provider => provider.GetRequiredService<Channel<PlayCount>>().Writer);
            services.AddScoped(provider => provider.GetRequiredService<Channel<PlayCount>>().Reader);

            services.AddScoped<IPlayCountProducer, PlayCountProducer>();
            services.AddScoped<IPlayCountConsumer, PlayCountConsumer>();

            services.AddScoped<PersistPlayCountSubcriber>();
            services.AddScoped<IPlayCountStream, PlayCountStream>(serviceProvider =>
            {
                var persistPlayCountSubcriber = serviceProvider.GetService<PersistPlayCountSubcriber>();
                var logger = serviceProvider.GetService<ILogger<PlayCountStream>>();

                var playCountStream = new PlayCountStream(logger);
                playCountStream.Subscribe(nameof(PersistPlayCountSubcriber), persistPlayCountSubcriber.Persist);

                return playCountStream;
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                      .AddCookie(options =>
                      {
                          options.ExpireTimeSpan = TimeSpan.FromDays(1);
                          options.LoginPath = "/login";
                          options.SlidingExpiration = true;
                          options.Events.OnValidatePrincipal = context =>
                          {
                              var expiredDateClaim = context.Principal.Claims.First(c => c.Type == "expired_date");
                              if (DateTime.TryParse(expiredDateClaim?.Value, out var expiredDate))
                              {
                                  if (expiredDate >= DateTime.UtcNow)
                                  {
                                      return Task.CompletedTask;
                                  }
                              }

                              context.RejectPrincipal();
                              return Task.CompletedTask;
                          };
                      });

            var jsonSetting = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddScoped(serviceProvider => {
                var librespotApi = new RestClient("https://api.t4ils.dev") { JsonSerializerSettings = jsonSetting }.For<ILibrespotApi>();
                return librespotApi;
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/hc");
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            var dashboardOptions = new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new HangfireDashboardAuthorizationFilter()
                }
            };

            app.UseHangfireDashboard(options: dashboardOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve", port: 8080);
                }
            });

            JobStorage.Current.GetMonitoringApi().PurgeProcessingJobs();

            BackgroundJob.Enqueue<IPlayCountConsumer>(consumer => consumer.ListenAsync(CancellationToken.None));
            RecurringJob.AddOrUpdate<PlayCountProducerJob>(job => job.RunAsync(), Cron.Daily());
        }
    }

    //public class RevokeAuthenticationEvents : CookieAuthenticationEvents
    //{
    //    public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    //    {
    //        var expiredDateClaim = context.Principal.Claims.First(c => c.Type == "expired_date");
    //        if (DateTime.TryParse(expiredDateClaim?.Value, out var expiredDate))
    //        {
    //            if (expiredDate >= DateTime.UtcNow)
    //            {
    //                return Task.CompletedTask;
    //            }
    //        }

    //        context.RejectPrincipal();
    //        return Task.CompletedTask;
    //    }
    //}
}
