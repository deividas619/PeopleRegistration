using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PersonInformation> PeopleInformation { get; set; }
        public virtual DbSet<ResidencePlace> ResidencePlaces { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-one key between PersonInformation and ResidencePlace
            modelBuilder.Entity<PersonInformation>()
                .HasOne(e => e.ResidencePlace)
                .WithOne(e => e.PersonInformation)
                .HasForeignKey<PersonInformation>(e => e.ResidencePlaceId);
            // Mapping user role enum number to string value
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}
