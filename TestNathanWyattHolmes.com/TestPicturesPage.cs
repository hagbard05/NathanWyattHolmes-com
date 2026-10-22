using NathanWyattHolmesComPages;

namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class TestPicturesPage : SeleniumTestBase
	{
		[TestMethod]
		public void PicturesPageLoadsWithGalleryImagesAndCategoryLinks()
		{
			var page = new PicturesPage(Driver);

			Assert.AreEqual("Pictures Mostly Taken by Nathan Holmes", Driver.Title.Trim());
			Assert.IsTrue(page.GalleryImages.Count > 0, "Expected the Pictures page to contain gallery images.");
			Assert.IsTrue(page.BestFriendsLink.GetAttribute("href")!.EndsWith("Pictures/Dogs/PicturesOfDogs.htm", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(page.NewYorkLink.GetAttribute("href")!.EndsWith("Pictures/NewYorkVacationPictures/PicturesOfNewYork.htm", StringComparison.OrdinalIgnoreCase));
		}

		[TestMethod]
		public void BestFriendsMenuLinkOpensDogsGallery()
		{
			var page = new PicturesPage(Driver);
			page.ClickBestFriendsLink();

			Assert.IsTrue(Driver.Url.EndsWith("PicturesOfDogs.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected the Best Friends gallery, but current URL was {Driver.Url}.");
		}

		[TestMethod]
		public void NewYorkMenuLinkOpensNewYorkGallery()
		{
			var page = new PicturesPage(Driver);
			page.ClickNewYorkLink();

			Assert.IsTrue(Driver.Url.EndsWith("PicturesOfNewYork.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected the New York gallery, but current URL was {Driver.Url}.");
		}
	}
}
