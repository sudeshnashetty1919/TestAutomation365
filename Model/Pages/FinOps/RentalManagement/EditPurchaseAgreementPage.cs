using dynamics365accelerator.Model.Components.Dialogs.DeviceManagement;
using dynamics365accelerator.Model.Components.Sections.LineDetails;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class EditPurchaseAgreementPage : BaseEditOrderPage<EditPurchaseAgreementPage>
    {
        public EditPurchaseAgreementPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public CreateReleaseOrderDialog ClickNewReleaseOrder()
        {
            TopMenu().ClickCRUDButtonInGroup("New", "Release order");

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public EditPurchaseAgreementPage StoreCreatedPurchaseOrderNumber(out string poNumber)
        {
            // TODO: should find a better way to do this
            poNumber = GetMessage(Should.Contain, "has been created")
                .Text
                .Split("Vendor account:")[1]
                .Split(" ")[4];
            Log().DataCreated("Purchase order", poNumber);
            return this;
        }

        public EditPurchaseAgreementPage StorePurchaseAgreementNumber(out string purchaseAgreementNumber)
        {
            var number = "";
            SuppressLogs(() =>
            {
                number = GetOrderNumber();
            });
            purchaseAgreementNumber = number;
            Log().DataCreated("Purchase agreement number", purchaseAgreementNumber);

            return this;
        }

        public EditPurchaseAgreementPage SetStatus(string status)
        {
            var input = driver.FindElement(By.CssSelector("input[name='LineViewHeader_AgreementState']"));

            SetInput(
                "Status",
                input,
                status
            );

            return this;
        }

        public LineDetailsSection<EditPurchaseAgreementPage> GetLineDetailsSection() => new(this, report);

        //Completed

    }
}