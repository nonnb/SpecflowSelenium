using CommonSeleniumNuGet;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Banking.WebApp.SpecflowTests.StepDefinitions
{
    [Binding]
    public sealed class TransferStepDefinitions
    {
        private ScenarioContext _scenarioContext;
        private readonly Random _random = new Random();

        public TransferStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeFeature]
        public static void ScenarioSetup()
        {
            var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            SeleniumHelper.Driver = new ChromeDriver(path + @"\drivers\");
        }

        [AfterFeature]
        public static void FeatureCleanup()
        {
            SeleniumHelper.Driver.Quit();
        }

        [Given("I create a new '(.*)' Bank Account for account holder '(.*)'")]
        public void GivenANewBankAccount(string accountType, string accountHolder)
        {
            SeleniumHelper.NavigateTo("https://localhost:7105/BankAccounts/Create");

            var accountName = $"Test-{Guid.NewGuid().ToString("N")}";
            var bankAccountNumber = $"{_random.Next(100000):D5}";
            SeleniumHelper.SafeTypeById("AccountHolder", accountName);
            SeleniumHelper.SafeTypeById("BankAccountNumber", bankAccountNumber);
            SeleniumHelper.SafeTypeById("AccountTypeId", accountType);
            SeleniumHelper.SafeClickById("Create");
            Assert.NotNull(SeleniumHelper.Driver.FindElement(By.XPath($"//td[contains(text(), '{accountName}')]")), 
                "Could not create new bank account");
            _scenarioContext["LastBankAccountNumber"] = bankAccountNumber;
        }

        [Given("I attempt transfer of funds from account '(.*)'")]
        public void GivenIAttemptTransfer(string fromAccountNumber)
        {
            SeleniumHelper.NavigateTo($"https://localhost:7105/BankAccounts/Transfer/{fromAccountNumber}");
            _scenarioContext["LastBankAccountNumber"] = fromAccountNumber;
        }

        [When("I transfer (.*) from Account '(.*)' to this account")]
        public void WhenITransferFromAccountToThisAccount(decimal amount, string fromAccountNumber)
        {
            SeleniumHelper.NavigateTo($"https://localhost:7105/BankAccounts/Transfer/{fromAccountNumber}");
            SeleniumHelper.SafeTypeById("ToBankAccountId", (string)_scenarioContext["LastBankAccountNumber"]);
            SeleniumHelper.ClearById("Amount");
            SeleniumHelper.SafeTypeById("Amount", amount.ToString("#.00"));
            SeleniumHelper.SafeClickById("Transfer");
        }

        [When("I type '(.*)' into the '(.*)' input")]
        public void WhenITypeIntoTheInput(string value, string input)
        {
            SeleniumHelper.ClearById(input);
            SeleniumHelper.SafeTypeById(input, value);
            SeleniumHelper.SafeTypeById(input, Keys.Tab);
        }

        [Then("this Account balance should be (.*)")]
        public void ThenThisAccountBalanceShouldBe(decimal amount)
        {
            SeleniumHelper.NavigateTo($"https://localhost:7105/BankAccounts/Details/{_scenarioContext["LastBankAccountNumber"]}");
            var balanceEl = SeleniumHelper.WaitForElementVisible(By.Id("Balance"));
            Assert.AreEqual(amount.ToString("#.00"), balanceEl.Text);
        }

        [Then("I should see the validation error '(.*)'")]
        public void IShouldSeeValidationError(string error)
        {
            var validationError = SeleniumHelper.WaitForElementVisible(By.XPath($"//span[contains(text(), '{error}')]"));
            Assert.IsNotNull(validationError);
        }
    }
}