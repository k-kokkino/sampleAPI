namespace Kkokkino.SampleAPI.Persistence.Model;

using System.Collections.Generic;

using Kkokkino.SampleAPI.Persistence.Abstractions;
using Kkokkino.SampleAPI.Persistence.Joins;

public class Movie : Entity<long>
{
  public string Title { get; set; }

  public double Rating { get; set; }

  public IReadOnlyCollection<MovieRenters> MovieRenters { get; set; }
    = new List<MovieRenters>();
}
