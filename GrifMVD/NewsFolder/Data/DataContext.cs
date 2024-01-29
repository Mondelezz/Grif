
using GrifMVD.NewsFolder.Models;
using Microsoft.EntityFrameworkCore;

namespace GrifMVD.NewsFolder.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GrifMVD;Username=postgres;Password=26032005");
        }
        public DbSet<NewsDb> News { get; set; }
        public DbSet<PhotosDb> Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhotosDb>()
                .HasOne(n => n.NewsDb)
                .WithMany(n => n.Photos)
                .HasForeignKey(key => key.NewsDbID);
        }
    }
}
