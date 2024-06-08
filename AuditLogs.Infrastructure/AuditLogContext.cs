using AuditLogs.Domain.Entities;
using Infrastructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Infrastructure
{
    public partial class AuditLogContext : AppDbContext<AuditLogContext>
    {
        public AuditLogContext(DbContextOptions<AuditLogContext> options)
           : base(options)
        {
        }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditLogAction> AuditLogActions { get; set; }
        public DbSet<EntityChange> EntityChanges { get; set; }
        public DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }
        public void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
        }
    }
}
