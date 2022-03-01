namespace Kkokkino.SampleAPI.Web.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Kkokkino.SampleAPI.Persistence;
  using Kkokkino.SampleAPI.Web.Helpers;
  using Kkokkino.SampleAPI.Web.Service;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  [Route("api/service")]
  public class ServiceController : BaseController<ServiceController>
  {
    private BatchEmailService service;

    public ServiceController(IEnumerable<IHostedService> services, PersistenceContext persistenceContext,
      ILogger<ServiceController> logger)
      : base(persistenceContext, logger)
    {
      service = services.OfType<BatchEmailService>().Single();
    }

    [HttpGet("")]
    public async Task<ActionResult> Trigger()
    {
      service.Trigger();
      return NoContent();
    }
  }
}
