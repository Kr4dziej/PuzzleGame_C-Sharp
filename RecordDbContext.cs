using Microsoft.EntityFrameworkCore;

public class RecordDbContext : DbContext
{
    public RecordDbContext(DbContextOptions<RecordDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Record> Records { get; set; }
}
