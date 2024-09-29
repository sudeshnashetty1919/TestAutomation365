using System;
using System.Collections.Generic;
using System.IO;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Model.Pages;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Aventstack.ExtentReports.Reporter;
using dynamics365accelerator.Model;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Support.Utils;
using AventStack.ExtentReports;
using dynamics365accelerator.Tests.DataProvider;
using System.Linq;
using dynamics365accelerator.Support.Driver;
using AventStack.ExtentReports.MarkupUtils;



namespace dynamics365accelerator.Tests
{
    [TestFixture]
    public class BaseTests
    {
        protected Dictionary<string, object> objectContainer = new();

        protected static ExtentReports extent;

        protected string description;

        [ThreadStatic]
        protected static Report t_report;

        [ThreadStatic]
        protected static IWebDriver t_driver;

        [ThreadStatic]
        protected static UserContextData t_userContextData;

        static BaseTests()
        {
            var reporter = new ExtentHtmlReporter(TestContext.CurrentContext.TestDirectory);
            //Force display of typically collapsed nodes.
            reporter.config.CSS = ".collapse {display: inline; }";
            extent = new ExtentReports();
            extent.AttachReporter(reporter);
        }

        [OneTimeSetup]

        public static void BeforeAll()
        {
            //IDriverFactory.SetUpDriverManager(config.GetSection("SELENIUM_BROWSER").values);
        }

        [SetUp]
        public void SetUp()
        {
            t_report = new Report(extent);
            var config = EnvConfig.Get();
            var gridUrl = config.GetSection("SELENIUM_GRID_URL").Value;
            var browser = config.GetSection("SELENIUM_BROWSER").Value;
            var headless = bool.Parse(config.GetSection("SELENIUM_HEADLESS").Value);
            var implictWait = int.Parse(config.GetSection("SELENIUM_WAIT").Value);
            var url = config.GetSection("FB_URL").Value;
            t_driver = IDriverFactory.BuildDriver(browser,gridUrl,headless);
            t_driver.Manage().Timeouts().implictWait = TimeSpan.FromMilliSeconds(implictWait);
            t_driver.Manage().Window.Maximize();
            t_driver.Navigate().GoToUrl(url);

            List<UserContextData> userContextData = UserContextDataProvider.ReadUserData();
            Login(userContextData,config,t_report);

        }

        [TearDown]
        public void TearDown()
        {
            t_report.BeginSection("Result");
            if(TestContext.CurrentContext.Result.Outcome == ResultState.Success)
            {
                t_report.Test.Pass("All assertion passed.");
            }
            else if(TestContext.CurrentContext.Result.Outcome == ResultState.Inconclusive)
            {
                t_report.Test.Skip($"Test not run: {TestContext.CurrentContext.Result.Message}"); 
            }
            else
            {
                var file = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{TestContext.CurrentContext.Test.Name}-screenshot.png");
                TestContext.WriteLine($"Saving screenshot to {file}");
                ((ITakeScreenshot)t_driver).GetScreenshot().SaveAsFile(file);
                t_report.Test.AddScreenCaptureFromPath(file);
                //clean up the message and replace newlines with divs to ensure that the message is formatted correctly.
                var messageLines = TestContext.CurrentContext.Result.Message
                    .Split("\n")
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0)
                    .Select(line => $"<div>{line}</div>")
                    .ToList();
                var stackTrace = TestContext.CurrentContext.Result.StackTrace
                    .Split("\n")
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0)
                    .Select(line => $"<div><tt>{line}</tt></div>")
                    .ToList(); 
                t_report.Test.Fail(string.Join("",messageLines.Concat(stackTrace)));       
            }
            extent.Flush();
            t_driver.Quit();
        }

        protected T Open<T>() where T : BaseObject<T>
        {
            return PageActions.ConstructPage<T>(t_driver, t_report);
        }
        protected T TestData<T>(string name, T testdata)
        {
            return t_report.LogTestData($"<b>{name}</b>",testData);
        }
        private void Login(List<UserContextData> userContextData,IConfiguration config, Report report)
        {
            SetUserContextData(userContextData, config, report);

            if(String.IsNullOrEmpty(t_userContextData.EncryptedPassword))
               throw new UserConfigurationException($"password is not set for PrincipalUSer '{t_userContexrData.principalName}' - please run the password generator for this user");
            report.Data.Log(Status.Info, $"<b>USer</b> :{t_userContextData.FormatFortLog()}");
            
            if(t_userContextData.PrincipalName.Contains("@sdmg.co.nz"))
            {
	            Open<MicrosoftLoginEnterEmailPage>()
		            .SetEmail(t_userContextData.PrincipalName)
		            .ClickNext()
		            .SetPassword(Cryptography.DecryptString(t_userContextData.EncryptedPassword))
		            .ClickSignIn()
		            .ClickStaySignedInYes();
            }
            else
            {
                throw new Exception("A FB email should be used to log in - steps may need to be implemented if this is changed.");
            }
        }
        private void SetUserContextData(List<USerContextData> userContextData, IConfiguration config, Report report)
        {
	        var testCategories = TestContext.CurrentContext.Test.Properties["category"];
	        foreach(object testCategory in testCategories)
            {
	            if(testCategory.ToString().ToLower().StartsWith("runasrole"))
	            { 
		            string userRole=testCategory.ToString().Replace("runasrole:","",true, Sytem.Globalization.CultureInfo.CurrentCulture);
		            //report.Test.Log(Status.Info,$"Attempting to get user context data for Test Category '{testCategory}'.");
		
		            if(userContextData.Where(user => user.Role.ToLower() == userRole.ToLower()).Any())
		            {
			            t_usesrContextData = userContextData.First(user => user.Role.ToLower() == userRole.ToLower());
			            //report.Test.Log(Status.Info, $"Retrieved user context data for test Category '{testCategory}' -> mapped it to PrincipalUSer '{t_userContextData.PrincipalName}' with role '{t_userContextData.Role}'.");
			            retun;
		            }
		            else
			            throw new UserConfigurationException($"Cannot map Test Category role '{userRole}' to any PrincipalUser in the UserContextData.json config files - please check configuration.");
	            }
            }
            SetDefaultUSerContextData(userContextData, config, report);
        }
        private void SetDefaultContextData(List<UserContextData> userContextData, IConfiguration config, Report report)
        {
	        //report.Test.Log(Status.Info, "No Test Category prefixed with 'RunAsRole' found - getting user context data from environment variable 'FB_USERNAME'.");
	        if(String.IsNullOrEmpty(config.GetSection("FB _DEFAULT_USERNAME").Value)) throw new UserConfigurationException("Environment variable 'D365_DEFAULT_USERNAME' is missing or empty.");
	        
            if(userContextData.Where(user => user.PrincipalName.ToLower() == config.GetSection("FB_DEFAULT_USERNAME").Value.ToLower()).Any())
	        {
		        t_userContextData = userContextData.First(user => user.PrincipalName.ToLower() == config.GetSection("FB_DEFAULT_USERNAME").Value.ToLower());
	        }
	        else
		        throw new UserConfigurationException("Cannot find default user with PrincipalName'"+ config.GetSection("FB_DEFAULT_USERNAME").Value + "' in the UserContextData.json config file - please check configuration.");
        }
        public class UserConfigurationException : Exception
        {
	        public USerConfigurationException(string message): base(message)
            {

            }
        } 
        
        

    }
}
    