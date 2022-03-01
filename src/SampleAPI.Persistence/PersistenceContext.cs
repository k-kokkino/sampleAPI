namespace Kkokkino.SampleAPI.Persistence;

using Kkokkino.SampleAPI.Persistence.Identity;
using Kkokkino.SampleAPI.Persistence.Joins;
using Kkokkino.SampleAPI.Persistence.Model;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class PersistenceContext : IdentityDbContext<SampleApiUser, SampleApiRole, Guid>
{
  public PersistenceContext(DbContextOptions<PersistenceContext> options)
    : base(options)
  {
  }

  public DbSet<Person> People { get; private set; } // DbSet einai pinakas

  public DbSet<Movie> Movies { get; private set; }

  public DbSet<MovieRenters> MovieRenters { get; private set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder?.Entity<Person>(e =>
    {
      // e.ToTable("people_dim");
      e.HasKey(e => e.Id);
      // e.HasKey(e => new{Id=e.Id,Name=e.Name}); COMPOSITE KEY
      e.Property(e => e.Name).HasMaxLength(30); // Customize property
    });

    builder?.Entity<MovieRenters>(e =>
    {
      var personMovieRenterKey = "MovieId";
      var moviePersonRenterKey = "PersonId";

      e.HasOne(e => e.Movie)
        .WithMany()
        .HasForeignKey(personMovieRenterKey);

      e.HasOne(e => e.Renter)
        .WithMany()
        .HasForeignKey(moviePersonRenterKey);

      e.HasKey(personMovieRenterKey, moviePersonRenterKey);
    });
  }
}
