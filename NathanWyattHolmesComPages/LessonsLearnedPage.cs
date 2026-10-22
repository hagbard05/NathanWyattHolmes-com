using OpenQA.Selenium;

namespace NathanWyattHolmesComPages
{
	public class LessonsLearnedPage : SitePage
	{
		public LessonsLearnedPage(IWebDriver driver) : base(driver, "OnSoftwareQualityEngineering.html")
		{
		}

		public IWebElement Heading => driver.FindElement(By.TagName("h1"));
		public IWebElement HomeLink => driver.FindElement(By.CssSelector("#menuHomeTop a.TopLevelMenuItem"));
		public IWebElement PicturesLink => driver.FindElement(By.CssSelector("#menuPersonalTop a.TopLevelMenuItem"));
		public IWebElement PicturesMenuLink => driver.FindElement(By.CssSelector("#menuPersonal a[href='Pictures.htm']"));

		public void ClickPicturesLink()
		{
			PicturesLink.Click();
		}
	}
}
