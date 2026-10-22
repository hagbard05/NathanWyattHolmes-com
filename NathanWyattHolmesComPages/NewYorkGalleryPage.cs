using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NathanWyattHolmesComPages
{
	public class NewYorkGalleryPage : SitePage
	{
		public NewYorkGalleryPage(IWebDriver driver) : base(driver, "Pictures/NewYorkVacationPictures/PicturesOfNewYork.htm")
		{
		}

		public IWebElement HomeLink => driver.FindElement(By.Id("HomeLink"));
		public IWebElement PicturesLink => driver.FindElement(By.Id("PicturesLink"));
		public IWebElement BestFriendsLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='../Dogs/PicturesOfDogs.htm']"));
		public IWebElement NewYorkLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='PicturesOfNewYork.htm']"));
		public IReadOnlyCollection<IWebElement> GalleryImages => driver.FindElements(By.CssSelector(".grid img"));

		public void ClickBestFriendsLink()
		{
			new Actions(driver).MoveToElement(PicturesLink).Perform();
			BestFriendsLink.Click();
		}
	}
}
