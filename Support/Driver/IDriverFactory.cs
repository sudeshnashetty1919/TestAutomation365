using System;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Events;
//dynamics365accelerator == dynamics365accelerator
namespace dynamics365accelerator.Support.Driver
{
    public interface IDriverFactory
    {
        public IWebDriver GetDriver(bool headless);
        
        public DriverOptions GetOptions(bool headless);

        public static IWebDriver BuildDriver(string browser, string? gridUrl = null, bool headless = true)
        {
            var factoryType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetInterface(typeof(IDriverFactory).ToString()) !=null)
                .FirstOrDefault(type => type.Name.ToLower().Contains(browser))
                ?? throw new Exception($"Browser {browser} not supported");

            var driverFactory = Activator.CreateInstance(factoryType) as IDriverFactory; 
            var driver = gridUrl !=null && gridUrl.Length > 0 ?
                new RemoteWebDriver(new Uri(gridUrl), driverFactory.GetOptions(headless)) :
                driverFactory.GetDriver(headless);
            return new DriverListenersDecorator().Decorate(new EventFiringWebDriver(driver));       


        }
        //Completed
    }

}