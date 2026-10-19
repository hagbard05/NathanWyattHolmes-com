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
			driver.Navigate().GoToUrl("https://www.nathanwyattholmes.com");
			Thread.Sleep(0); 
		}

		public IWebElement menuProfessional => driver.FindElement(By.CssSelector("#menuProfessionalTop .TopLevelMenuItem"));
		public IWebElement lnkLessionsLearnedInSoftwareTesting => driver.FindElement(By.CssSelector("a[href='OnSoftwareQualityEngineering.html']"));
		public IWebElement lnkLinkedIn => driver.FindElement(By.CssSelector("a[href='https://www.linkedin.com/in/nathan-wyatt-holmes/?isSelfProfile=false']"));

		public void ClickLessionsLearnedInSoftwareTesting()
		{
			Actions actions = new Actions(driver);
			actions.MoveToElement(menuProfessional).Perform();
			lnkLessionsLearnedInSoftwareTesting.Click();
		}

		public void ClickLinkedIn()
		{
			Actions actions = new Actions(driver);
			actions.MoveToElement(menuProfessional).Perform();
			lnkLinkedIn.Click();
		}
	}
}
