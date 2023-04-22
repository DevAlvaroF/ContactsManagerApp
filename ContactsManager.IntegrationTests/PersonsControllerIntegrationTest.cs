using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace CRUDTests
{
	public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
	{
		private HttpClient _httpClient;
		public PersonsControllerIntegrationTest(CustomWebApplicationFactory customWebApplicationFactory)
		{
			_httpClient = customWebApplicationFactory.CreateClient();

		}

		#region Index
		[Fact]
		public async Task Index_ToReturnView()
		{
			// Arrange ============================================

			// Act ================================================

			// Get response Message
			HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/Persons/Index");

			// Assert =============================================

			// Check if response is 2xx
			httpResponseMessage.Should().BeSuccessful();

			// Read body as string
			string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

			// Load HTML DOM (Agility Pack)
			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(responseBody);

			// Traverse the DOM
			HtmlNode document = html.DocumentNode;

			// Get Table with Checker class
			document.QuerySelectorAll("table.persons-check").Should().NotBeNull();
		}
		#endregion
	}
}
