namespace Kkokkino.SampleAPI.Web.Controllers;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Kkokkino.SampleAPI.Persistence;
using Kkokkino.SampleAPI.Persistence.Identity;
using Kkokkino.SampleAPI.Web.Helpers;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

[Route("api/account")]
public class AccountController : BaseController<AccountController>
  {

  private const string PROTECTIONSTRING = "UserManagement";

  public AccountController(PersistenceContext persistenceContext, ILogger<AccountController> logger, IDataProtectionProvider dataProtectionProvider,
    UserManager<SampleApiUser> userManager, RoleManager<SampleApiRole> roleManager, IConfiguration configuration)
      : base(persistenceContext, logger)
  {
    DataProtector = dataProtectionProvider.CreateProtector(PROTECTIONSTRING);
    UserManager = userManager;
    RoleManager = roleManager;
    Configuration = configuration;
  }

  private IDataProtector DataProtector { get; }

  private UserManager<SampleApiUser> UserManager { get; }

  private RoleManager<SampleApiRole> RoleManager { get; }

  private IConfiguration Configuration { get; }
  [HttpPost("register")]
  public async Task<ActionResult> Register(RegisterDto dto)
  {
    var user = new SampleApiUser
    {
      UserName = dto.Username,
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
    };
    var result = await UserManager.CreateAsync(user, dto.Password);
    if (result.Succeeded)
    {
      return Ok();
    }
    else
    {
      return BadRequest(result.Errors);
    }
  }

  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    var user = await UserManager.FindByNameAsync(loginDto.Username)
      ?? await UserManager.FindByEmailAsync(loginDto.Username);
    if (user == null || !(await UserManager.CheckPasswordAsync(user, loginDto.Password)))
    {
      return NotFound("No such user or wrong password.");
    }

    var roles = await UserManager.GetRolesAsync(user);

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.UserName)
    };

    claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

    var jwtToken = new JwtSecurityToken(
      issuer: Configuration["JWT:Issuer"],
      audience: Configuration["JWT:Audience"],
      signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])), SecurityAlgorithms.HmacSha256),
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(Configuration["JWT:ExpirationInMinutes"])));

    var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
    var dto = new UserDto(user.UserName, user.Email, token);

    return Ok(dto);
  }
}

public record LoginDto(string Username, string Password);
public record UserDto(string Username, string Email, string Token);
public record RegisterDto(string Username, string Email, string Password, string FirstName, string LastName);
