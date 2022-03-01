namespace Kkokkino.SampleAPI.Web.Helpers.Extensions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Threading.Tasks;

  public static class LinqExtensions
  {
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition,
      Expression<Func<TSource, bool>> predicate)
      => condition
        ? source.Where(predicate)
        : source;
  }
}
