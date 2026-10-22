using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NathanWyattHolmesComPages
{
	public class PicturesPage : SitePage
	{
		public PicturesPage(IWebDriver driver) : base(driver, "Pictures.htm")
		{
		}

		public IWebElement HomeLink => driver.FindElement(By.Id("HomeLink"));
		public IWebElement PicturesLink => driver.FindElement(By.Id("PicturesLink"));
		public IWebElement BestFriendsLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='Pictures/Dogs/PicturesOfDogs.htm']"));
		public IWebElement NewYorkLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='Pictures/NewYorkVacationPictures/PicturesOfNewYork.htm']"));
		public IReadOnlyCollection<IWebElement> GalleryImages => driver.FindElements(By.CssSelector(".picturesTable img"));

		public void ClickBestFriendsLink()
		{
			OpenPicturesMenu();
			BestFriendsLink.Click();
		}

		public void ClickNewYorkLink()
		{
			OpenPicturesMenu();
			NewYorkLink.Click();
		}

		private void OpenPicturesMenu()
		{
			new Actions(driver).MoveToElement(PicturesLink).Perform();
		}
	}
}
