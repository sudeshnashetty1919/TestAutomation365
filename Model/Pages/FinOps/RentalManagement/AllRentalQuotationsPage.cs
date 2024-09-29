using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class AllRentalQuotationsPage : BaseCRUDPage<AllRentalQuotationsPage>
    {
        public AllRentalQuotationsPage(IWebDriver driver, Report report) : base(driver, report)
        {}

        public CreateRentalQuotationDialog<AllRentalQuotationsPage> clickNewQuotation()
        {
            TopMenu().ClickNew();

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                new EditRentalOrderPage(driver, report),
                report
            );

            //Completed
        }
    }
}