namespace Kkokkino.SampleAPI.Web.Helpers;

using Kkokkino.SampleAPI.Persistence;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
public abstract class BaseController<T> : ControllerBase
  where T : BaseController<T>
{

  protected BaseController(PersistenceContext persistenceContext, ILogger<T> logger)
  {
    Logger = logger;
    Ctx = persistenceContext;
  }

  protected PersistenceContext Ctx { get; }

  protected ILogger<T> Logger { get; }
}
