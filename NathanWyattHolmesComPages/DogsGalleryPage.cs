using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NathanWyattHolmesComPages
{
	public class DogsGalleryPage : SitePage
	{
		public DogsGalleryPage(IWebDriver driver) : base(driver, "Pictures/Dogs/PicturesOfDogs.htm")
		{
		}

		public IWebElement HomeLink => driver.FindElement(By.Id("HomeLink"));
		public IWebElement PicturesLink => driver.FindElement(By.Id("PicturesLink"));
		public IWebElement BestFriendsLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='PicturesOfDogs.htm']"));
		public IWebElement NewYorkLink => driver.FindElement(By.CssSelector("#PicturesMenu a[href='../NewYorkVacationPictures/PicturesOfNewYork.htm']"));
		public IReadOnlyCollection<IWebElement> GalleryImages => driver.FindElements(By.CssSelector(".grid img"));

		public void ClickNewYorkLink()
		{
			new Actions(driver).MoveToElement(PicturesLink).Perform();
			NewYorkLink.Click();
		}
	}
}
