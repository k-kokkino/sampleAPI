namespace Kkokkino.SampleAPI.Persistence.Abstractions
{
  using System;

  public abstract class Entity<TKey> : IEntity<TKey>
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
    public TKey Id { get; set; }
  }
}
