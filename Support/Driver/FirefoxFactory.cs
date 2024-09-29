using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace dynamics365accelerator.Support.Driver
{
    public class FirefoxFactory : IDriverFactory
    {
        public DriverOptions GetOptions(bool headless)
        {
            var options = new FirefoxOptions();
            if (headless) options.AddArguments("--headless");
            return options;
        }

        public IWebDriver GetDriver(bool headless)
        {
            return new FirefoxDriver(GetOptions(headless) as FirefoxOptions);

        }
        //Completed
    }
}