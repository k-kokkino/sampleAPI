namespace Kkokkino.SampleAPI.Persistence.Joins;

using Kkokkino.SampleAPI.Persistence.Model;

public class MovieRenters
{
  // private long PersonId { get; set; }

  // private long MovieId { get; set; }

  public Person Renter { get; set; }

  public Movie Movie { get; set; }
}
