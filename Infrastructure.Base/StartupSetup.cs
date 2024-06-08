using Application.Base.BackgroundJob;
using Application.Base.Services;
using Domain.Base.Tracing;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Base.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base
{
    public static class StartupSetup
    {
        public static IServiceCollection AddBackgroundJobService(this IServiceCollection services, string dbConnection)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(dbConnection,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        UsePageLocksOnDequeue = true,
                        DisableGlobalLocks = true,
                        JobExpirationCheckInterval = TimeSpan.FromSeconds(3)
                    }));

            // Add the processing server as IHostedService
            services.AddHangfireServer(opts => {
                opts.WorkerCount = 1;
            });
            GlobalJobFilters.Filters.Add(new ProlongExpirationTimeAttribute());

            services.AddSingleton<Application.Base.BackgroundJob.IBackgroundJob, DefaultBackgroundJob>();
            services.AddSingleton<IRecurringJob, DefaultRecurringJob>();
            return services;
        }
        public static IServiceCollection AddBaseInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserService, ExternalUserService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<CorrelationIdOptions>();
            services.AddSingleton<ICorrelationIdProvider, AspNetCoreCorrelationIdProvider>();
            services.AddTransient<CorrelationIdMiddleware>();
            return services;
        }
        public static void UseBackgroundJobDashboard(this IApplicationBuilder app, string url = "/mnbgjobs")
        {
            app.UseHangfireDashboard(url, new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });
        }
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
