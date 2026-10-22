using NathanWyattHolmesComPages;

namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class TestNewYorkGalleryPage : SeleniumTestBase
	{
		[TestMethod]
		public void NewYorkGalleryLoadsWithHomeAndPicturesNavigation()
		{
			var page = new NewYorkGalleryPage(Driver);

			Assert.AreEqual("Image Gallery", Driver.Title.Trim());
			Assert.IsTrue(page.GalleryImages.Count > 0, "Expected the New York gallery to contain images.");
			Assert.IsTrue(page.HomeLink.Displayed);
			Assert.IsTrue(page.PicturesLink.Displayed);
		}

		[TestMethod]
		public void BestFriendsMenuLinkOpensDogsGallery()
		{
			var page = new NewYorkGalleryPage(Driver);
			page.ClickBestFriendsLink();

			Assert.IsTrue(Driver.Url.EndsWith("PicturesOfDogs.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected the Best Friends gallery, but current URL was {Driver.Url}.");
		}
	}
}
