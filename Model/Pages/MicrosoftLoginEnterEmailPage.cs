using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages
{
    public class MicrosoftLoginEnterEmailPage : BasePage<MicrosoftLoginEnterEmailPage>
    {
        public MicrosoftLoginEnterEmailPage(IWebDriver driver, Report report) : base(driver, report)
        { }

        public MicrosoftLoginEnterEmailPage SetEmail(string email)
        {
            new Wait(driver, 15)
                .UntilDisplayed(By.CssSelector("[name='loginfmt']"))
                .SendKeys(email);
        return this;
        }

        public MicrosoftLoginEnterPasswordPage ClickNext()
        {
            new Wait(driver, 10)
                .UntilDisplayed(By.CssSelector("input[value='Next']"))
                .Click();
            return new(driver, report);
        }   
        
    }
}
