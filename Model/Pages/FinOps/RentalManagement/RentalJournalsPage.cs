using System.Linq;
using System.Threading;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class RentalJournalsPage : BaseJournalPage<RentalJournalsPage>
    {
        public RentalJournalsPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public string GetCustomerAccount()
        {
            var customerAccount = GetJournalsDataTable().GetColumnValues("Customer account").Single();
            Log().Value("Customer account", customerAccount);
            return customerAccount;
        }

        public string GetCustomerAccount(string name)
        {
            var customerAccount = GetJournalsDataTable()
                .GetCell("Name", name, "Customer account")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");
            Log().Value($"Customer account (Line name: '{name}')", customerAccount);
            return customerAccount;
        }

        public string GetRentalDevice()
        {
            var rentalDevice = GetTransactionsDataTable().GetColumnValues("Rental device").Single();
            Log().Value("Rental device", rentalDevice);
            return rentalDevice;
        }

        public DataTable<RentalJournalsPage> GetJournalsDataTable()
        {
            return new(
                new Wait(driver, 5).UntilDisplayed(Selectors.Table("Rental journals")),
                this,
                report,
                "Rental journals"
            );
        }

        public DataTable<RentalJournalsPage> GetTransactionsDataTable()
        {
            return new(
                new Wait(driver, 5).UntilDisplayed(Selectors.Table("Rental journal transactions")),
                this,
                report,
                "Rental journal transactions"
            );
        }

        public RentalJournalsPage ClickPrint(string nameToPrint)
        {
            Log().Click("Print");
            new Wait(driver, 2).Retry(d => d.FindElement(Selectors.Button("Print")).Click());

            // hover another element to avoid tooltip
            new Actions(driver)
                .MoveToElement(driver.FindElement(Selectors.Input("Pickup date")))
                .Build()
                .Perform();

            Thread.Sleep(1000);

            Log().Click("Print > Quotation");
            new Wait(driver, 5).Retry(d => d
                .FindElements(By.CssSelector("[data-dyn-role='MenuItem']"))
                .First(e => e.Text.Equals(nameToPrint))
                .Click()
            );

            WaitForOperationProcessing();
            WaitForReportRetrieval();

            return this;
        }
    
    }
}
