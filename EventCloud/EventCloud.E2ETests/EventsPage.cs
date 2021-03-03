using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace EventCloud.E2ETests
{
    [TestClass]
    public class EventsPage
    {
        private RemoteWebDriver _driver;

        private WebDriverWait _wait;

        private TestContext _testContext { get; set; }

        [TestInitialize]
        public void WebDriverInitialize()
        {
            var sauceOptions = new Dictionary<string, object>();

            sauceOptions["username"] = "USERNAME";
            sauceOptions["accessKey"] = "ACCESS_KEY";
            //sauceOptions["tunnelIdentifier"] = "TUNNEL_IDENTIFIER";
            //sauceOptions["build"] = "BUILD_NAME";

            var browserOptions = new SafariOptions();
            browserOptions.PlatformName = "macOS 10.15";
            browserOptions.BrowserVersion = "latest";
            browserOptions.AddAdditionalCapability("sauce:options", sauceOptions);

            _driver = new RemoteWebDriver(new Uri("https://ondemand.saucelabs.com:443/wd/hub"), browserOptions);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        [TestCleanup]
        public void WebDriverCleanup()
        {
            var result = _testContext.CurrentTestOutcome == UnitTestOutcome.Passed
                ? "passed"
                : "failed";
            _driver.ExecuteScript($"sauce:job-result={result}");
            _driver.Quit();
        }

        [TestMethod]
        public void ShowsCancelledEvents_WhenIncludeCancelledEventsCheckboxIsChecked()
        {
            _driver.ExecuteScript($"sauce:job-name={_testContext.TestName}");
            LogStep("Opening login page:");
            _driver.Url = "http://eventcloud.aspnetboilerplate.com/account/login?TenantId=1";
            LogStep("Signing in:");
            _wait.Until(driver => driver.FindElements(By.Id("LoginButton")).Any());
            //_driver.FindElements(By.CssSelector("form input"))[0].SendKeys("john");
            //_driver.FindElements(By.CssSelector("form input"))[1].SendKeys("123qwe");
            var loginButton = _driver.FindElement(By.Id("LoginButton"));
            _driver.ExecuteScript("arguments[0].click();", loginButton);
            LogStep("Opening sidebar:");
            _wait.Until(driver => driver.FindElements(By.ClassName("bars")).Any());
            var sidebarToggle = _driver.FindElement(By.ClassName("bars"));
            _driver.ExecuteScript("arguments[0].click();", sidebarToggle);
            LogStep("Opening Events page:");
            var eventsLink = _driver.FindElements(By.CssSelector(".menu li"))[1].FindElement(By.TagName("a"));
            eventsLink.Click();
            _driver.ExecuteScript("arguments[0].click();", sidebarToggle);
            LogStep("Selecting cancelled events:");
            var includeCanceledEventsCheckbox = _driver.FindElement(By.Id("includeCanceledEvents"));
            _driver.ExecuteScript("arguments[0].click();", includeCanceledEventsCheckbox);
            Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
            LogStep("Asserting cancelled events are present:");
            Assert.IsTrue(_driver.FindElements(By.ClassName("bg-red")).Any());
        }

        private void LogStep(string stepDescription)
        {
            _driver.ExecuteScript($"sauce:context={stepDescription}");
        }
    }
}