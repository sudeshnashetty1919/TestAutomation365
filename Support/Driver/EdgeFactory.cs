using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace dynamics365accelerator.Support.Driver
{
    public class EdgeFactory : IDriverFactory
    {
        public DriverOptions GetOptions(bool headless)
        {
            var options = new EdgeOptions();
            if (headless) options.AddArguments("--headless");
            return options;
        }

        public IWebDriver GetDriver(bool headless)
        {
            return new EdgeDriver(GetOptions(headless) as EdgeOptions);

        }
        //Completed
    }
}