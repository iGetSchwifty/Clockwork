using System;
using Microsoft.EntityFrameworkCore;

namespace Clockwork.API.Models
{
    public class ClockworkContext : DbContext
    {
        public DbSet<CurrentTimeQuery> CurrentTimeQueries { get; set; }

        public ClockworkContext(DbContextOptions<ClockworkContext> options) : base(options)
        {
        }
    }

    public class CurrentTimeQuery
    {
        // Would add attributes but I usually like to create my db from a schema and then scafold my model with dotnet ef dbcontext scaffold also love sqlserver not sqlite
        public int CurrentTimeQueryId { get; set; }
        public DateTime Time { get; set; }
        public string ClientIp { get; set; }
        public DateTime UTCTime { get; set; }
        public string RequestedTimezone { get; set; }
    }
}
