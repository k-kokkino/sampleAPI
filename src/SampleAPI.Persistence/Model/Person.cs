namespace Kkokkino.SampleAPI.Persistence.Model;

using Kkokkino.SampleAPI.Persistence.Abstractions;

public class Person : Entity<long>
{
  private string name = string.Empty;

  public string Name
  {
    get => name;
    set
    {
      name = value;
      NormalizedName = name.ToUpperInvariant();
    }
  }

  public string? NormalizedName { get; private set; } = string.Empty;
}
