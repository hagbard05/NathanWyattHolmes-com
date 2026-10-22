using NathanWyattHolmesComPages;

namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class TestLessonsLearnedPage : SeleniumTestBase
	{
		[TestMethod]
		public void LessonsLearnedPageLoadsWithHomeAndPicturesNavigation()
		{
			var page = new LessonsLearnedPage(Driver);

			Assert.AreEqual("Software Quality Assurance", page.Heading.Text);
			Assert.AreEqual("home.html", new Uri(page.HomeLink.GetAttribute("href")!).AbsolutePath.Split('/').Last());
			Assert.AreEqual("Pictures.htm", page.PicturesMenuLink.GetAttribute("href")!.Split('/').Last());
		}

		[TestMethod]
		public void PicturesLinkOnLessonsLearnedPageOpensPicturesPage()
		{
			var page = new LessonsLearnedPage(Driver);
			new OpenQA.Selenium.Interactions.Actions(Driver).MoveToElement(page.PicturesLink).Perform();
			page.PicturesMenuLink.Click();

			Assert.IsTrue(Driver.Url.EndsWith("Pictures.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected the Pictures page, but current URL was {Driver.Url}.");
		}
	}
}
