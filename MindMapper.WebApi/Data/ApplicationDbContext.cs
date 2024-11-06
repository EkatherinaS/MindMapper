using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data.Entities;

namespace MindMapper.WebApi.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    
    public DbSet<Topic> Topics { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Topic>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Document>()
            .HasMany(e => e.Topics)
            .WithOne(e => e.Document)
            .HasForeignKey(d => d.DocumentId)
            .HasPrincipalKey(x => x.Id);
    }
}