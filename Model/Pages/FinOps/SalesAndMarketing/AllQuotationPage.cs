using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Model.Components.Dialogs.SalesAndMarketing;
using dynamics365accelerator.Support.Utils;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class AllQuotationPage : BaseCRUDPage<AllQuotationPage>
    {
        public AllQuotationPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public CreateQuotationDialog ClickNewQuotation()
        {
            TopMenu().ClickCRUDButton("New");

            return new(
                new Wait(driver, 5).ForDialogPopup(),
                this,
                new EditQuotationPage(driver, report),
                report
            );
        }

        public EditQuotationPage CreateQuotation(Quotation quotation)
        {
            var editQuotationPage = ClickNewQuotation()
                .SetAccountType(quotation.AccountType)
                .SetCustomerAccount(quotation.CustomerAccount)
                .ClickYes()
                .ExpandSection("General")
                .SetCustomerReference(quotation.CustomerReference)
                .ClickOk();
                string? quotationOrder = null;
                SuppressLogs(() =>
                {
                   quotationOrder = editQuotationPage.GetOrderNumber();
                           
                });
            Log().DataCreated("Quotation Number", quotationOrder);
            Log().Value("Quotation Number", quotationOrder);

            return editQuotationPage;
        }

        //Completed
    }
}