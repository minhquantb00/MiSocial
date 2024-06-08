using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base
{
    public abstract class AppDbContext<TDbContext> : DbContext
        where TDbContext : DbContext
    {
        public IServiceProvider ServiceProvider { get; set; }
        private IAuditingManager _auditingManager => ServiceProvider.GetRequiredService<IAuditingManager>();
        private IEntityHistoryHelper EntityHistoryHelper => ServiceProvider.GetRequiredService<IEntityHistoryHelper>();
        private ILogger<AppDbContext<TDbContext>> Logger => ServiceProvider.GetRequiredService<ILogger<AppDbContext<TDbContext>>>();
        public AppDbContext(DbContextOptions<TDbContext> options)
           : base(options)
        {
        }
    }
}
