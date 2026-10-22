using NathanWyattHolmesComPages;

namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class TestDogsGalleryPage : SeleniumTestBase
	{
		[TestMethod]
		public void DogsGalleryLoadsWithHomeAndPicturesNavigation()
		{
			var page = new DogsGalleryPage(Driver);

			Assert.AreEqual("Image Gallery", Driver.Title.Trim());
			Assert.IsTrue(page.GalleryImages.Count > 0, "Expected the Best Friends gallery to contain images.");
			Assert.IsTrue(page.HomeLink.Displayed);
			Assert.IsTrue(page.PicturesLink.Displayed);
		}

		[TestMethod]
		public void NewYorkMenuLinkOpensNewYorkGallery()
		{
			var page = new DogsGalleryPage(Driver);
			page.ClickNewYorkLink();

			Assert.IsTrue(Driver.Url.EndsWith("PicturesOfNewYork.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected the New York gallery, but current URL was {Driver.Url}.");
		}
	}
}
