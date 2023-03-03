using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using CommonSeleniumNuGet;

namespace Banking.WebApp.SeleniumTests
{
    public class AccountCreationTests
    {
        private IWebDriver _driver;
        private readonly Random _random = new Random();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            _driver = new ChromeDriver(path + @"\drivers\");
            SeleniumHelper.Driver = _driver;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        // Se can get very messy very quickly unless you build your own abstraction layer
        [Test]
        public void CreateAccountIsClicked_ShouldNavigateToCreateAccountPage()
        {
            _driver.Navigate().GoToUrl("https://localhost:7105/BankAccounts");
            IWebElement createAccount = _driver.FindElement(By.XPath("//a[text()='Create']"));
            createAccount.Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(1000));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            wait.Until(d => d.Url.ToLowerInvariant().Contains("create"));

            Assert.IsTrue(_driver.FindElement(By.XPath("//input[@value='Create']")).Displayed);
        }

        // Can be multiple abstraction layers, e.g. Common NuGet, but then system specific abstractions with opinions
        // about e.g. what kind of UI framework, modal popups, growl notifications, etc.
        [TestCase("Cheque")]
        [TestCase("Savings")]
        public void CreatingNewAccount_ShouldCreateNewAccount(string accountType)
        {
            SeleniumHelper.NavigateTo("https://localhost:7105/BankAccounts/Create");

            var accountName = $"Test-{Guid.NewGuid().ToString("N")}";
            SeleniumHelper.SafeTypeById("AccountHolder", accountName);
            SeleniumHelper.SafeTypeById("BankAccountNumber", $"{_random.Next(100000):D5}");
            SeleniumHelper.SafeTypeById("AccountTypeId", accountType);
            SeleniumHelper.SafeClickById("Create");

            Assert.NotNull(SeleniumHelper.Driver.FindElement(By.XPath($"//td[contains(text(), '{accountName}')]")));
        }
    }
}