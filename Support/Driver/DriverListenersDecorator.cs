using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using System;

namespace dynamics365accelerator.Support.Driver
{
    public class DriverListenersDecorator
    {
        internal IWebDriver Decorate(EventFiringWebDriver driver)
        {
            driver.ElementClicking += new EventHandler<WebElementEventArgs>((sender, e) =>
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", e.Element);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('style','background: yellow; border: 2px solid red;');", e.Element);
                TestContext.Out.WriteLine($"[{TestContext.CurrentContext.Test.Name}] Attempting to click element at " + DateTime.Now.ToString("hh:mm:ss.fff tt"));
            });
            driver.ElementClicked += new EventHandler<WebElementEventArgs>((sender, e) =>
            {
                try{((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('style','background: grey; border: 2px solid red;');", e.Element); }
                catch(Exception) {}
                TestContext.Out.WriteLine($"[{TestContext.CurrentContext.Test.Name}] clicked element at " + DateTime.Now.ToString("hh:mm:ss.fff tt"));
            
            });
            driver.FindingElement += new EventHandler<FindElementEventArgs>((sender, e) =>
                TestContext.Out.WriteLine($"[{TestContext.CurrentContext.Test.Name}] Trying to find element {e.FindMethod} at " + DateTime.Now.ToString("hh:mm:ss.fff tt"))
            );
            driver.FindElementCompleted += new EventHandler<FindElementEventArgs>((sender, e) =>
                TestContext.Out.WriteLine($"[{TestContext.CurrentContext.Test.Name}] found element {e.FindMethod} at " + DateTime.Now.ToString("hh:mm:ss.fff tt"))
            );
            driver.ElementValueChanged += new EventHandler<WebElementEventArgs>((sender, e) =>
            {
                try
                {
                    if(e.Element.GetAttribute("type").Equals("password"))
                       TestContext.Out.WriteLine($"Sent text: ****** at " + DateTime.Now.ToString("hh:mm:ss.fff tt"));
                    else
                       TestContext.Out.WriteLine($"Sent text: {e.value} at " + DateTime.Now.ToString("hh:mm:ss.fff tt"));   
                }
                catch{}
            });
            return driver;

        }
        //Completed
    }
    
}