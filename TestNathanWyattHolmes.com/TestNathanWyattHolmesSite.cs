using NathanWyattHolmesComPages;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class TestNathanWyattHolmesSite
	{
		static IWebDriver driver;
		HomePage homePage;

		public TestContext TestContext { get; set; }

		[ClassInitialize]
		public static void ClassInit(TestContext context)
		{
			var options = new ChromeOptions();

			// Core arguments required for Linux CI agents
			//options.AddArgument("--headless=new");          // Runs without a GUI
			options.AddArgument("--no-sandbox");             // Bypasses container security boundaries
			options.AddArgument("--disable-dev-shm-usage");  // Overcomes resource limits in Linux

			Console.WriteLine("Initializing headless ChromeDriver for MSTest execution.");
			driver = new ChromeDriver(options);
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(6);
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
			driver.Quit();
		}

		[TestInitialize]
		public void TestInit()
		{
			TestContext.WriteLine("Opening the landing page of Nathan Wyatt Holmes' website.");
			homePage = new HomePage(driver);
		}

		[TestCleanup]
		public void TestCleanup()
		{
			if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
			{
				TakeScreenshot();
				AttachPageSource();
			}
		}

		#region Test Methods
		[TestMethod]
		public void OpenLandingPage()
		{
			TestContext.WriteLine("Verifying the landing page title.");
			Assert.AreEqual("Nathan Holmes - Software Developer Engineer in Test - AI Engineer", driver.Title);
		}

		[TestMethod]
		public void ClickLinkedIn()
		{
			TestContext.WriteLine("Clicking the LinkedIn link on the landing page.");
			homePage.ClickLnkLinkedIn();
			Assert.IsTrue(driver.Url.Contains("linkedin.com", StringComparison.OrdinalIgnoreCase),
				$"Expected to navigate to LinkedIn, but current URL was {driver.Url}.");
		}

		[TestMethod]
		public void ClickLessonsLearned()
		{
			TestContext.WriteLine("Clicking the 'Lessons Learned' link on the landing page.");
			homePage.ClickLnkLessionsLearnedInSoftwareTesting();
			Assert.IsTrue(driver.Url.EndsWith("OnSoftwareQualityEngineering.html", StringComparison.OrdinalIgnoreCase),
				$"Expected to open the Lessons Learned page, but current URL was {driver.Url}.");
		}

		[TestMethod]
		public void ClickResume()
		{
			TestContext.WriteLine("Clicking the resume link on the landing page.");
			Assert.IsTrue(homePage.lnkResume.GetAttribute("href")?.EndsWith("Resume_NathanHolmes.docx", StringComparison.OrdinalIgnoreCase),
				"Expected the Resume menu link to point to Resume_NathanHolmes.docx.");
			homePage.ClickLnkResume();
		}

		[TestMethod]
		public void VerifyContactLink()
		{
			TestContext.WriteLine("Verifying the Contact link on the landing page.");
			Assert.AreEqual("mailto:NathanHolmes@NathanWyattHolmes.com", homePage.lnkContact.GetAttribute("href"));
		}

		[TestMethod]
		public void ClickHomeFromLessonsLearned()
		{
			TestContext.WriteLine("Opening the Lessons Learned page and returning home from the top menu.");
			homePage.ClickLnkLessionsLearnedInSoftwareTesting();
			homePage.ClickLnkHome();
			Assert.IsTrue(driver.Url.EndsWith("home.html", StringComparison.OrdinalIgnoreCase),
				$"Expected to return to the home page, but current URL was {driver.Url}.");
			Assert.AreEqual("Nathan Holmes - Software Developer Engineer in Test - AI Engineer", driver.Title.Trim());
		}

		[TestMethod]
		public void ClickPicturesFromPersonalMenu()
		{
			TestContext.WriteLine("Opening the Pictures page from the Personal top-menu dropdown.");
			homePage.ClickLnkPictures();
			Assert.IsTrue(driver.Url.EndsWith("Pictures.htm", StringComparison.OrdinalIgnoreCase),
				$"Expected to open the Pictures page, but current URL was {driver.Url}.");
			Assert.AreEqual("Pictures Mostly Taken by Nathan Holmes", driver.Title.Trim());
		}
		#endregion

		private void TakeScreenshot()
		{
			try
			{
				string fileName = $"{TestContext.TestName}_Screenshot.png";
				string filePath = Path.Combine(TestContext.TestRunResultsDirectory, fileName);
				((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(filePath);
				TestContext.AddResultFile(filePath);
			}
			catch (Exception ex)
			{
				TestContext.WriteLine($"Error saving screenshot: {ex.Message}");
			}
		}

		void AttachPageSource()
		{
			TestContext.WriteLine("Attempting to save and attach the complete page as an MHTML archive.");
			try
			{
				// Send raw command via Chromium DevTools Protocol (CDP) to take a snapshot
				var parameters = new Dictionary<string, object> { { "format", "mhtml" } };

				// This returns the full MHTML structural string enclosing CSS, JS, and image assets
				var response = ((ChromeDriver)driver).ExecuteCdpCommand("Page.captureSnapshot", parameters) as Dictionary<string, object>;

				if (response != null && response.TryGetValue("data", out var mhtmlData))
				{
					string mhtmlContent = mhtmlData.ToString();

					// Define the path inside the MSTest execution directory using the .mhtml extension
					string fileName = $"{TestContext.TestName}_CompletePage.mhtml";
					string filePath = Path.Combine(TestContext.TestRunResultsDirectory, fileName);

					// Save the full archive to disk
					File.WriteAllText(filePath, mhtmlContent, System.Text.Encoding.UTF8);

					// Attach the complete page directly to the individual MSTest result
					TestContext.AddResultFile(filePath);
				}
			}
			catch (Exception ex)
			{
				TestContext.WriteLine($"Failed to save complete MHTML page: {ex.Message}");
			}
		}

		public void Dispose()
		{
			driver?.Dispose();
		}
	}
}
