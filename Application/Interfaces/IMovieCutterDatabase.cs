using Domain.BusinessModels.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces
{
    public interface IMovieCutterDatabase
    {
        public DatabaseFacade Database { get; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public Task MigrateAsync();
        public Task<int> SaveChangesAsync();
    }
}
