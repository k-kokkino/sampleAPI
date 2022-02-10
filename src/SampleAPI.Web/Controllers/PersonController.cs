namespace Kkokkino.SampleAPI.Web.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kkokkino.SampleAPI.Persistence;
using Kkokkino.SampleAPI.Web.Helpers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[Route("api/person")]
public class PersonController : BaseController<PersonController>
{
  private PersonDto? person;

  public PersonController(PersistenceContext persistenceContext, ILogger<PersonController> logger)
    : base(persistenceContext, logger)
  {
  }

  [Authorize] // REQUIRES AUTH (in our case it's jwt)
  [HttpGet("{id}")]
  public async Task<ActionResult<PersonDto>> PersonGet(long id)
  {
    //  var result = await Ctx.People
    //    .Where(x => x.Id == id)
    //    .SingleOrDefaultAsync();

    var result = await Ctx.People
        .SingleOrDefaultAsync(x => x.Id == id);

    if (person == null)
    {
      Logger.LogWarning("Person with id {Id} does not exist!", id);
      return NotFound();
    }

    return Ok(person);
  }

  /// <summary>
  /// Post Method.
  /// </summary>
  /// <returns></returns>
  /// <response code="409">Conflict.</response>
  [Authorize]
  [HttpPost("")]
  public async Task<ActionResult<PersonDto>> PersonPost(string name)
  {
    if (person is null)
    {
      person = new PersonDto(0, name);
      // return Ok(person); den epistrefoume 200 ok alla 201 created
      return CreatedAtAction(nameof(PersonGet), new { id = person.Id }, person);
    }
    else
    {
      return Conflict();
    }
  }

  /// <summary>
  /// Cancellation token showcase
  /// </summary>
  /// <param name="token"></param>
  /// <returns></returns>
  [HttpGet("")]
  public async Task<ActionResult<List<PersonDto>>> GetAllPeople(CancellationToken token)
  {
    Logger.LogCritical("Enter Method");
    var query = Ctx.People;

      // .Where(person => Ctx.MovieRenters.Any(e => e.Renter.Id == person.Id)); // In order to return all people who have rented at least one movie
    try
    {
      await Task.Delay(TimeSpan.FromSeconds(10), token);
      var result = await query.ToListAsync(token);
      return Ok(result);
    }
    catch (OperationCanceledException)
    {
      Logger.LogCritical("Task was cancelled");
    }

    return BadRequest();
  }

  [HttpPut("")]
  public async Task<ActionResult<PersonDto>> PersonPut(string name)
  {
    person = person is null ? new PersonDto(0, name) : person with { Name = name };
    return Ok(person);
  }

  [HttpDelete("")]
  public async Task<ActionResult<PersonDto>> PersonDelete()
  {
    person = null;
    return NoContent();
  }

  /// <summary>
  /// Method response 404.
  /// </summary>
  /// <returns></returns>
  /// <response code="404">Not Found.</response>
  [HttpGet("notfound")]
  public async Task<ActionResult<PersonDto>> NotFoundExample()
  {
    return NotFound("Person Not Found");
  }

}

public record PersonDto(long Id, string Name);
