using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using SeleniumExtras.WaitHelpers;
using System.Linq;

namespace dynamics365accelerator.Support.Utils
{
    ///<summary>
    ///wait is a customer wrapper over 'WebDriverwait'.
    ///It's required because c# selenium's 'WebDriverwait'does not correctly handle and retry on exceptions
    ///even with the use of 'IgnoreExceptionTypes'.
    ///  The wait utilities 'until' and 'Retry' in this class correctly retry on exceptions.    
    ///</summary>

    public class Wait
    {
        protected IWebDriver driver;
        private readonly int timeout;
        private readonly int sleepInterval;

        public Wait(IWebDriver driver,int timeout = 30,int sleepInterval = 500)
        {
            this.driver = driver;
            this.timeout = timeout;
            this.sleepInterval = sleepInterval;

        }

        private WebDriverWait GetWait()
        {
            return new WebDriverWait(
                new Systemlock(),
                driver,
                TimeSpan.FromSeconds(timeout),
                TimeSpan.FromMilliseconds(sleepInterval)

            );
        }

        public IWebElement Until(Func<IWebDriver, IWebElement> expectedCondition)
        {
            return GetWait().Until(d =>
            {
                try
                {
                    return expectedCondition(d);
                }
                catch
                {
                    return null;  
                }
            })!;
        }

        public void Retry(Action<IWebDriver> action)
        {
            GetWait().Until(d =>
            {
                try
                {
                    action(d);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public bool Until(Func<IWebDriver, bool> expectedCondition)
        {
            Retry(d =>
            {
                if(!expectedCondition(d)) throw new Exception();
            });
            return true;
        }

        public IWebElement UntilDisplayed(By locator)
        {
            return Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public IWebElement UntilClickable(IWebElement element)
        {
            return Until(ExpectedConditions.ElementToBeClickable(element));
        }

        public IWebElement UntilClickable(By locator)
        {
            return Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        public bool UntilNotDisplayed(By locator)
        {
            return Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }

        public bool UntilNotExisting(By locator)
        {
            return Until(d => d.FindElements(locator).Count == 0);
        }

        public void ForBlocking()
        {
            //The below function probably doesn't do anything as the Blocking element is present always but only visible when used.
            //However, given that this function is used everwhere, it's not being tweaked until it needs to be.
            //Be aware that this function does not wait for the element to appear.
            Until(ExpectedConditions.ElementExists(Selectors.Blocking()));
            UntilNotDisplayed(Selectors.Blocking());
        }

        public IWebElement ForDialogPopup() => Until(d => d.FindElements(Selectors.Dialog()).First(e => e.Displayed));
        public IWebElement ForDialogPopup(string heading) => UntilDisplayed(Selectors.Dialog(heading));
        public IWebElement ForLightbox() => UntilDisplayed(Selectors.Lightbox());
        public static T WithImplicitWait<T>(IWebDriver driver, TimeSpan implicitWait, Func<IWebDriver, T> action)
        {
            var oldImplicitWait = driver.Manage().Timeouts().ImplicitWait;
            try
            {
                driver.Manage().Timeouts().ImplicitWait = implicitWait;
                return action(driver);

            }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = oldImplicitWait;
            }
        }
        //Completed

}
}