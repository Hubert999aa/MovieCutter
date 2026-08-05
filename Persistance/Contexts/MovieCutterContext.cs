using Application.Interfaces;
using Domain.BusinessModels.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Contexts
{
    public class MovieCutterContext : DbContext, IMovieCutterDatabase
    {
        public MovieCutterContext() { }
        public MovieCutterContext(DbContextOptions<MovieCutterContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Operation> Operations { get; set; }

        public Task MigrateAsync()
        {
            return Database.MigrateAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>(opt =>
            {
                opt.HasKey(p => p.IdProfile);
                opt.Property(p => p.IdProfile)
                   .ValueGeneratedOnAdd();

                opt.Property(p => p.Name)
                   .HasMaxLength(100)
                   .IsRequired();
            });

            modelBuilder.Entity<Source>(opt =>
            {
                opt.HasKey(p => p.IdSource);
                opt.Property(p => p.IdSource)
                   .ValueGeneratedOnAdd();

                opt.Property(p => p.Name)
                   .HasMaxLength(100)
                   .IsRequired();

                opt.Property(p => p.BaseUrl)
                   .HasMaxLength(200)
                   .IsRequired();

                opt.Property(p => p.SourceType)
                   .HasConversion<int>();

                opt.HasOne(p => p.Profile)
                   .WithMany(p => p.Sources)
                   .HasForeignKey(p => p.IdProfile)
                   .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<Operation>(opt =>
            {
                opt.HasKey(p => p.IdOperation);
                opt.Property(p => p.IdOperation)
                   .ValueGeneratedOnAdd();

                opt.Property(p => p.VideoName)
                   .HasMaxLength(100)
                   .IsRequired();

                opt.Property(p => p.VideoExtension)
                   .HasMaxLength(5)
                   .IsRequired();

                opt.Property(p => p.OperationStatus)
                   .HasConversion<int>();

                opt.Property(p => p.OperationType)
                   .HasConversion<int>();

                opt.Property(p => p.VideoProcess)
                   .HasConversion<int>();
            });
        }
    }
}
