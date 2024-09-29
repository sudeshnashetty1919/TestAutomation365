using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using NUnit.Framework;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using WebDriverManager;


namespace dynamics365accelerator.Support.Driver
{
    public class ChromeFactory : IDriverFactory
    {
        public DriverOptions GetOptions(bool headless)
        {
            var options = new ChromeOptions();
            options.AddArguments("--disable-gpu",
                       "--window-size=1920,1080",
                       "--ignore-certificate-errors" 
                       );
            if (headless) options.AddArguments("--headless");
            var prefs = new Dictionary<string, object>
            {
                {"download.default_directory", TestContext.CurrentContext.TestDirectory}
            };
            options.AddUserProfilePreference("download.default_directory", TestContext.CurrentContext.TestDirectory);
            return options;
        }

        public IWebDriver GetDriver(bool headless)
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            return new ChromeDriver((ChromeOptions)GetOptions(headless));
        }

        public class ChromeWithPrefs : ChromeOptions
        {
            public Dictionary<string, object> prefs {get; set;}
        }
      //Completed
    }
}