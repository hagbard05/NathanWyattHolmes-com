using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestNathanWyattHolmes.com
{
	public abstract class SeleniumTestBase
	{
		protected IWebDriver Driver { get; private set; } = null!;

		[TestInitialize]
		public void InitializeDriver()
		{
			var options = new ChromeOptions();
			options.AddArgument("--headless=new");
			options.AddArgument("--no-sandbox");
			options.AddArgument("--disable-dev-shm-usage");
			Driver = new ChromeDriver(options);
			Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(6);
		}

		[TestCleanup]
		public void CleanupDriver()
		{
			Driver?.Quit();
		}
	}
}
