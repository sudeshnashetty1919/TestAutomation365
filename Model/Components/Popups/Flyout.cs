using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;
using dynamics365accelerator.Model.Components.Dialogs.SalesAndMarketing;
using dynamics365accelerator.Model.Pages;
using dynamics365accelerator.Model.Pages.FinOps.InventoryManagement;
using dynamics365accelerator.Model.Pages.FinOps.RentalManagement;
using dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Popups
{
    public class Flyout<TParent> : BaseComponent<TParent, Flyout<TParent>>
        where TParent : BaseObject<TParent>
    {
        public Flyout(IWebElement rootElement, TParent parent, Report report, string displayName)
            : base(rootElement, parent, report, displayName)
        {
        }

        public TParent Click(string buttonName)
        {
            Log().Click(buttonName);
            Root().FindElement(Selectors.Button(buttonName)).Click();
            new Wait(driver).ForBlocking();
            return parent;
        }

        public TParent Click(string section, string buttonName)
        {
            Log().Click($"{section} > {buttonName}");
            // TODO: the section should be integrated into the selector
            return Click(buttonName);
        }

        public TResult ClickOpenPage<TResult>(string section, string buttonName)
            where TResult : BasePage<TResult>
        {
            Log().Click($"{section} > {buttonName}");
            // TODO: the section should be integrated into the selector
            return ClickOpenPage<TResult>(buttonName);
        }

        public TResult ClickOpenPage<TResult>(string buttonName)
            where TResult : BasePage<TResult>
        {
            Log().Click($"{buttonName}");
            Root().FindElement(Selectors.Button(buttonName)).Click();
            new Wait(driver).ForBlocking();
            return PageActions.ConstructPage<TResult>(driver, report);
        }

        public TResult ClickOpenDialog<TResult>(string buttonName)
            where TResult : BaseSinglePageDialog<TParent, TResult>
        {
            Log().Click($"{buttonName}");
            Root().FindElement(Selectors.Button(buttonName)).Click();
            new Wait(driver).ForBlocking();
            return PageActions.ConstructComponent<TResult, TParent>(
                new Wait(driver).ForDialogPopup(),
                parent,
                report);
        }

        public TResult ClickOpenDialog<TResult>(string section, string buttonName)
            where TResult : BaseSinglePageDialog<TParent, TResult>
        {
            Log().Click($"{section} > {buttonName}");
            // TODO: the section should be integrated into the selector
            return ClickOpenDialog<TResult>(buttonName);
        }

        // This must be specialized while we don't have a way to open binary dialogs from Flyouts
        // to review.
        public TransferOrderDialog ClickNewTransferOrder()
        {
            Log().Click("New Transfer Order");
            Root().FindElement(Selectors.Button("Transfer order")).Click();
            return new(
                new Wait(driver, 5).ForDialogPopup(),
                new EditSalesOrderPage(driver, report),
                new EditTransferOrderPage(driver, report),
                report
            );
        }

        public CreatePurchaseAgreementDialog ClickCreatePurchaseAgreement()
        {
            Log().Click("Create purchase agreement");
            Root().FindElement(Selectors.Button("Create purchase agreement")).Click();
            return new(
                new Wait(driver, 5).ForDialogPopup(),
                new EditRentalDevicePage(driver, report),
                new EditPurchaseAgreementPage(driver, report),
                report
            );
        }
        //Completed
    }
}