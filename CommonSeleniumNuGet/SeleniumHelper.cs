using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CommonSeleniumNuGet
{
    public static class SeleniumHelper
    {
        public static IWebDriver Driver;

        public static void NavigateTo(string url)
        {
            Driver.Navigate()
                .GoToUrl(url);
        }

        public static IWebElement WaitForElementVisible(By by, int milliseconds = -1)
        {
            var element = Driver.FindElement(by);
            return WaitForElementVisible(element);
        }

        public static IWebElement WaitForElementVisible(IWebElement element, int milliseconds = -1)
        {
            var wait = ConfigureWait();
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            wait.Until(_ => element.Displayed);
            return element;
        }

        public static void WaitForUrlChange(string urlContains, TimeSpan waitTime = default)
        {
            WaitFor(Driver, d => d.Url.ToLowerInvariant().Contains(urlContains.ToLowerInvariant()),
                waitTime == default
                    ? -1
                    : (int)waitTime.TotalMilliseconds);
        }

        public static bool WaitFor(this IWebDriver driver, Func<IWebDriver, bool> function, int milliseconds = -1)
        {
            return ConfigureWait(milliseconds).Until(d => function(d));
        }

        private static WebDriverWait ConfigureWait(int milliseconds = -1)
        {
            var wait = new WebDriverWait(Driver,
                TimeSpan.FromMilliseconds(milliseconds == -1
                    ? 1000
                    : milliseconds));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            return wait;
        }

        public static bool HasClass(this IWebElement el, string className)
        {
            return el.GetAttribute("class").Split(' ').Contains(className);
        }

        // Web browsers typically don't like it if you click on elements which aren't visible
        public static void ScrollIntoView(IWebElement el)
        {
            var action = new Actions(Driver);
            action.MoveToElement(el);
            action.Perform();
        }

        public static void SafeClickById(string id)
        {
            var el = Driver.FindElement(By.Id(id));
            ScrollIntoView(el);
            WaitForElementVisible(el);
            el.Click();
        }

        public static void SafeTypeById(string id, string keys)
        {
            var el = Driver.FindElement(By.Id(id));
            ScrollIntoView(el);
            el.SendKeys(keys);
        }

        public static void ClearById(string id)
        {
            var el = Driver.FindElement(By.Id(id));
            ScrollIntoView(el);
            el.Clear();
        }
    }
}