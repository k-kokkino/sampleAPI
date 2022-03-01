namespace Kkokkino.SampleAPI.Web.Controllers;

using Helpers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Persistence;
using Persistence.Model;

[Route("api/person")]
public class PersonController : BaseController<PersonController>
{
  public PersonController(PersistenceContext persistenceContext, ILogger<PersonController> logger)
    : base(persistenceContext, logger)
  {
  }

  [Authorize] // REQUIRES AUTH (in our case it's jwt)
  [HttpGet("{id}")]
  public async Task<ActionResult<PersonDto>> PersonGet(long id)
  {
    // var result = await Ctx.People
    //    .Where(x => x.Id == id)
    //    .SingleOrDefaultAsync();

    var result = await Ctx.People
      .SingleOrDefaultAsync(x => x.Id == id);

    if (result is null)
    {
      Logger.LogWarning("Person with id {Id} does not exist!", id);
      return NotFound();
    }

    return Ok(result);
  }

  [Authorize]
  [HttpPost("")]
  public async Task<ActionResult<PersonDto>> PersonPost(PersonDto personDto)
  {
    var existingPerson = await Ctx.People
      .SingleOrDefaultAsync(x => x.Id == personDto.Id);
    if (existingPerson is null)
    {
      var person = new Person();
      person.Name = personDto.Name;
      Ctx.Add(person);
      await Ctx.SaveChangesAsync();

      // return Ok(person); den epistrefoume 200 ok alla 201 created
      return CreatedAtAction(nameof(PersonGet), new {id = person.Id}, person);
    }

    return Conflict();
  }

  /// <summary>
  ///   Cancellation token showcase.
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
  public async Task<ActionResult<PersonDto>> PersonPut(PersonDto personDto)
  {
    var person = await Ctx.People.SingleOrDefaultAsync(x => x.Id == personDto.Id) ?? new Person();
    person.Name = personDto.Name;
    Ctx.Update(person);
    await Ctx.SaveChangesAsync();
    return Ok(personDto);
  }

  [HttpDelete("")]
  public async Task<ActionResult<PersonDto>> PersonDelete(PersonDto personDto)
  {
    var person = await Ctx.People.SingleOrDefaultAsync(x => x.Id == personDto.Id);
    if (person is not null)
    {
      Ctx.Remove(person);
      await Ctx.SaveChangesAsync();
    }

    return Ok();
  }

  /// <summary>
  ///   Method response 404.
  /// </summary>
  /// <returns></returns>
  /// <response code="404">Not Found.</response>
  [HttpGet("notfound")]
  public async Task<ActionResult<PersonDto>> NotFoundExample() => NotFound("Person Not Found");
}

public record PersonDto(long Id, string Name);
