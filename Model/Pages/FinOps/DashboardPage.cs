using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps
{
    public class DashboardPage : AppBasePage<DashboardPage>
    {
        public DashboardPage(IWebDriver driver, Report report)
            : base(driver, report)
        {
        }

        protected override IWebElement Root()
        {
            return new Wait(driver, 60).Until(d => d.FindElement(Selectors.Page("DefaultDashboard")));
        }
    }
}//Completed