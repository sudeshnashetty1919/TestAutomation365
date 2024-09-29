using dynamics365accelerator.Model.Pages.FinOps.ContractManagement;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class RentalInvoicingJournalPage : BaseCRUDPage<RentalInvoicingJournalPage>
    {
        public RentalInvoicingJournalPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("AMCollectionJournalTable"));
        }

        public JournalLinesPage ClickLines()
        {
            Log().Click("Lines");
            new Wait(driver, 5)
                .Until(d => d.FindElement(Selectors.Button("Lines")))
                .Click();

            return new(driver, report);
        }//Completed
    }
}