
using Infrastructure.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Infrastructure
{
    public static class StartupSetup
    {
        public static IServiceCollection AddAuditLogInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddBaseInfrastructure();
            services.AddDbContext<AuditLogContext>(options =>
                options.UseSqlServer(connectionString,
                sqlServerOptionsAction: options => {
                    options.EnableRetryOnFailure();
                    options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                }));

            return services;
        }
        public static void UseAuditLogInfrastructure(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AuditLogContext>();
                var loggerFactory = serviceScope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                context.Database.Migrate();
                AuditLogContextSeed.SeedAsync(context, loggerFactory).Wait();
            }
        }
    }
}
