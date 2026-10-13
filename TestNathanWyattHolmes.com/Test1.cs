using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace TestNathanWyattHolmes.com
{
	[TestClass]
	public sealed class Test1
	{
		IWebDriver driver = new ChromeDriver();

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
			// This method is called once for the test class, before any tests of the class are run.
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
			// This method is called once for the test class, after all tests of the class are run.
		}

		[TestInitialize]
		public void TestInit()
		{
			// This method is called before each test method.
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
	}
}
