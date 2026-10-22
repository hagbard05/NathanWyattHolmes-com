using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace NathanWyattHolmesComPages
{
	public class HomePage
	{
		IWebDriver driver;
		public HomePage(IWebDriver driver)
		{
			this.driver = driver;
			driver.Navigate().GoToUrl("http://www.nathanwyattholmes.com");
			Thread.Sleep(0); 
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.Until(d => d.Url.Contains("nathanwyattholmes.com"));
		}

		public IWebElement menuProfessional => driver.FindElement(By.CssSelector("#menuProfessionalTop .TopLevelMenuItem"));
		public IWebElement menuPersonal => driver.FindElement(By.CssSelector("#menuPersonalTop .TopLevelMenuItem"));
		public IWebElement lnkHome => driver.FindElement(By.CssSelector("#menuHomeTop .TopLevelMenuItem"));
		public IWebElement lnkPictures => driver.FindElement(By.CssSelector("#menuPersonal a[href='Pictures.htm']"));
		public IWebElement lnkLessionsLearnedInSoftwareTesting => driver.FindElement(By.CssSelector("a[href='OnSoftwareQualityEngineering.html']"));
		public IWebElement lnkLinkedIn => driver.FindElement(By.CssSelector("a[href='https://www.linkedin.com/in/nathan-wyatt-holmes/?isSelfProfile=false']"));
		public IWebElement lnkResume => driver.FindElement(By.CssSelector("a[href='Resume_NathanHolmes.docx']"));
		public IWebElement lnkContact => driver.FindElement(By.CssSelector("a[href='mailto:NathanHolmes@NathanWyattHolmes.com']"));

		public void ClickLnkHome()
		{
			lnkHome.Click();
		}

		public void ClickLnkPictures()
		{
			new Actions(driver).MoveToElement(menuPersonal).Perform();
			lnkPictures.Click();
		}

		public void ClickLnkLessionsLearnedInSoftwareTesting()
		{
			ClickMenuItem(this.lnkLessionsLearnedInSoftwareTesting);
		}

		public void ClickLnkLinkedIn()
		{
			ClickMenuItem(this.lnkLinkedIn);
		}

		public void ClickLnkResume()
		{
			ClickMenuItem(this.lnkResume);
		}

		public void ClickMenuItem( IWebElement menuItem)
		{
			Actions actions = new Actions(driver);
			actions.MoveToElement(menuProfessional).Perform();
			menuItem.Click();
		}
	}
}
