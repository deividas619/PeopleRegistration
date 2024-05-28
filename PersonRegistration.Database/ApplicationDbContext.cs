using Microsoft.EntityFrameworkCore;
using PersonRegistration.Shared.Entities;

namespace PersonRegistration.Database
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PersonInformation> PersonInformation { get; set; }
        public virtual DbSet<ResidencePlace> ResidencePlace { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonInformation>()
                .HasOne(e => e.ResidencePlace)
                .WithOne(e => e.PersonInformation)
                .HasForeignKey<PersonInformation>(e => e.ResidencePlaceId);
        }
    }
}
