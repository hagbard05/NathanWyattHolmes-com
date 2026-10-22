using OpenQA.Selenium;

namespace NathanWyattHolmesComPages
{
	public abstract class SitePage
	{
		private const string SiteRootUrl = "http://www.nathanwyattholmes.com/";

		protected IWebDriver driver;

		protected SitePage(IWebDriver driver, string pagePath)
		{
			this.driver = driver;
			driver.Navigate().GoToUrl(new Uri(new Uri(SiteRootUrl), pagePath));
		}
	}
}
