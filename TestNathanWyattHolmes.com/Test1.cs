using NathanWyattHolmesComPages;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class Test1
	{
		static IWebDriver driver;
		HomePage homePage;

		public Test1()
		{
		}

		public TestContext TestContext { get; set; }

		[AssemblyInitialize]
		public static void AssemblyInit(TestContext context)
		{
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
		}

		[ClassInitialize]
		public static void ClassInit(TestContext context)
		{
			driver = new ChromeDriver(); 
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
				TestContext.WriteLine($"❌Test '{TestContext.TestName}' failed.");
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

				try
				{
					string pageSource = driver.PageSource;
					string fileName = $"{TestContext.TestName}_PageSource.html";
					string filePath = Path.Combine(TestContext.TestRunResultsDirectory, fileName);

					File.WriteAllText(filePath, pageSource, System.Text.Encoding.UTF8);
					TestContext.AddResultFile(filePath);
				}
				catch (Exception ex)
				{
					TestContext.WriteLine($"Failed to attach page source: {ex.Message}");
				}
			}
		}

		[TestMethod]
		public void OpenLandingPage()
		{
			Assert.AreEqual("Nathan Holmes - Software Developer Engineer in Test - AI Engineer", driver.Title);
		}


		[TestMethod]
		public void ClickLinkedIn()
		{
			driver.FindElement(By.CssSelector("a[href='https://www.linkedin.com/in/nathan-wyatt-holmes/?isSelfProfile=false']")).Click();
			Thread.Sleep(0);
		}

		[TestMethod]
		public void ClickLessonsLearned()
		{
			homePage.ClickLessionsLearnedInSoftwareTesting();
			Thread.Sleep(0);
		}

		[TestMethod]
		public void MockInconclusive()
		{
			TestContext.WriteLine("This is a mock inconclusive test.");
			Assert.Inconclusive("This test is expected to be inconclusive.");
		}
	}
}
