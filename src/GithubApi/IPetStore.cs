namespace Kkokkino.GithubApi
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Refit;

  public interface IPetStore
  {
    [Get("/pet/findByStatus")]
    Task<List<Pet>> GetByStatus(PetStatus status);
  }

  public enum PetStatus
  {
    available,
    pending,
    sold,
  }
}
