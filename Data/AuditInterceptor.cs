using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using Tp3.Models;

namespace Tp3.Data
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditLogs(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || 
                           e.State == EntityState.Modified || 
                           e.State == EntityState.Deleted)
                .Where(e => e.Entity.GetType() != typeof(AuditLog)) // Don't audit the audit logs
                .ToList();

            foreach (var entry in entries)
            {
                var auditLog = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    EntityKey = GetEntityKey(entry),
                    Date = DateTime.Now
                };

                if (entry.State == EntityState.Modified)
                {
                    var changes = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            changes[property.Metadata.Name] = new
                            {
                                Original = property.OriginalValue,
                                Current = property.CurrentValue
                            };
                        }
                    }
                    auditLog.Changes = JsonSerializer.Serialize(changes);
                }
                else if (entry.State == EntityState.Added)
                {
                    var values = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    auditLog.Changes = JsonSerializer.Serialize(values);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var values = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    auditLog.Changes = JsonSerializer.Serialize(values);
                }

                context.Add(auditLog);
            }
        }

        private string GetEntityKey(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var keyProperties = entry.Properties.Where(p => p.Metadata.IsKey()).ToList();
            if (keyProperties.Any())
            {
                return string.Join(",", keyProperties.Select(p => p.CurrentValue?.ToString() ?? "null"));
            }
            return "Unknown";
        }
    }
}
