namespace Kkokkino.SampleAPI.Persistence.Identity;

using System;

using Kkokkino.SampleAPI.Persistence.Abstractions;

using Microsoft.AspNetCore.Identity;

public class SampleApiRole : IdentityRole<Guid>, IEntity<Guid>
{
}
