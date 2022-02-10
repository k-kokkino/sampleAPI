using System.Threading.Tasks;

using Kkokkino.SampleAPI.Persistence.Model;

using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json;

using Xunit;

namespace SampleAPI.Web.Tests
{
    public class UnitTest1
    {
    [Fact]
    public async Task Test1()
    {
      using var application = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder => { });
      var client = application.CreateClient();
      var person = await client.GetAsync("/api/person");
      var testvar = await person.Content.ReadAsStringAsync();
      var desertestvar = JsonConvert.DeserializeObject<Person[]>(testvar);

    }
  }
}
