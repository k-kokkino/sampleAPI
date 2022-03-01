namespace Kkokkino.SampleAPI.Persistence.Identity
{
  using System;

  using Kkokkino.SampleAPI.Persistence.Abstractions;

  using Microsoft.AspNetCore.Identity;

  public class SampleApiUser : IdentityUser<Guid>, IEntity<Guid>
  {
    [ProtectedPersonalData] // This attribute encrypts transparently
    public string FirstName { get; set; }

    [ProtectedPersonalData] public string LastName { get; set; }
  }
}
