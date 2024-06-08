using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Infrastructure
{
    public class AuditLogContextSeed
    {
        public static async Task SeedAsync(AuditLogContext dbContext,
             ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                dbContext.Database.Migrate();

                #region seeding data

                dbContext.Database.OpenConnection();


                dbContext.Database.CloseConnection();

                #endregion
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<AuditLogContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(dbContext, loggerFactory, retryForAvailability);
                }
            }
        }
    }
}
