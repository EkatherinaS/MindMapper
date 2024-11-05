using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data.Entities;

namespace MindMapper.WebApi.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<PdfParsingResult> PdfParsingResults { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PdfParsingResult>()
            .HasKey(p => p.Id);
    }
}