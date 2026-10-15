using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class Test1
	{
		static IWebDriver driver;

		public Test1()
		{
			driver = new ChromeDriver();
		}

		public TestContext TestContext { get; set; }

		[AssemblyInitialize]
		public static void AssemblyInit(TestContext context)
		{
			// This method is called once for the test assembly, before any tests are run.
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			// This method is called once for the test assembly, after all tests are run.
		}

		[ClassInitialize]
		public static void ClassInit(TestContext context)
		{
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
			driver.Navigate().GoToUrl("https://www.nathanwyattholmes.com");
		}

		[TestCleanup]
		public void TestCleanup()
		{
			if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
			{
				TestContext.WriteLine($"❌Test '{TestContext.TestName}' failed.");

				((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("screenshot.png");
				TestContext.AddResultFile("screenshot.png");
				
			}
			
		}

		[TestMethod]
		public void OpenLandingPage()
		{
			TestContext.WriteLine("Opening the landing page of Nathan Wyatt Holmes' website.");
			driver.Navigate().GoToUrl("https://www.nathanwyattholmes.com");
			Assert.AreEqual("Nathan Holmes - Software Developer Engineer in Test - AI Engineer", driver.Title);
		}


		[TestMethod]
		public void ClickLinkedIn()
		{
			driver.FindElement(By.CssSelector("a[href='https://www.linkedin.com/in/nathan-wyatt-holmes/?isSelfProfile=false']")).Click();
		}


		[TestMethod]
		public void MockTest3()
		{
			TestContext.WriteLine("This is a mock inconclusive test.");
			Assert.Inconclusive("This test is expected to be inconclusive.");
		}
	}
}
