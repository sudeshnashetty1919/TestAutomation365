using dynamics365accelerator.Model.Pages.FinOps;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages
{
    public class MicrosoftLoginEnterPasswordPage : BasePage<MicrosoftLoginEnterPasswordPage>
    {
        public MicrosoftLoginEnterPasswordPage(IWebDriver driver, Report report) : base(driver, report)
        { }

        public MicrosoftLoginEnterPasswordPage SetPassword(string password)
        {
            new Wait(driver, 10)
                .UntilDisplayed(By.CssSelector("[name='passwd']"))
                .SendKeys(password);
            return this;
        }

        public MicrosoftLoginEnterPasswordPage ClickSignIn()
        {
            new Wait(driver, 10)
                .UntilDisplayed(By.CssSelector("input[value='Sign in']"))
                .Click();
            return this;
        }

        public DashboardPage ClickStaySignedInYes()
        {
            driver.FindElement(By.CssSelector("input[value='Yes']")).Click();
            return new(driver, report);
        }
        
    }
}