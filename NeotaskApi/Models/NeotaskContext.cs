using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace NeotaskApi.Models
{
    public class NeotaskContext : DbContext
    {
        public NeotaskContext(DbContextOptions<NeotaskContext> options)
            : base(options)
        {
        }

        public DbSet<Neotask> Neotasks { get; set; } = null!;
    }
}