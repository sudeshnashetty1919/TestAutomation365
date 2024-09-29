using OpenQA.Selenium;
using dynamics365accelerator.Model.Pages;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Model.Components.Dialogs.DeviceManagement;
using System;
using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;

namespace dynamics365accelerator.Model.Components.Menus.SideMenus
{
    public class ModulesFlyout<TParent> : BaseComponent<TParent, ModulesFlyout<TParent>>
        where TParent : BaseObject<TParent>
    {
        public ModulesFlyout(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
        }

        public ModulesFlyout<TParent> ClickExpandAll()
        {
            Log().Click("Expand all");

            new Wait(driver).Retry(d =>
                Root().FindElement(By.ClassName("modulesFlyout-ExpandAll")).Click()
            );

            return this;
        }

        public T NavigateTo<T>(string linkText) where T : BasePage<T>
        {
            Log().Info($"Navigate to '<b>{linkText}</b>'");

            new Wait(driver, 5).Until(d => Root().FindElement(Selectors.ModuleLink(linkText))).Click();

            return PageActions.ConstructPage<T>(driver, report);
        }

        // The "rental browser" link opens a dialog - this function exists to model this special case.
        public RentalBrowserDialog<TParent> ClickRentalBrowser()
        {
            Log().Click("Modules > Rental management > Rental browser");

            new Wait(driver, 5).Until(d => Root().FindElement(Selectors.ModuleLink("Rental browser"))).Click();

            return new(
                new Wait(driver).ForDialogPopup(),
                parent,
                report
            );
        }

        public PeriodicInvoicingDialog<TParent> ClickCreateInvoicingJournal()
        {
            Log().Click("Modules > Contract management > Create invoicing journal");

            new Wait(driver, 5).Until(d => Root().FindElement(Selectors.ModuleLink("Create invoicing journal"))).Click();

            return new(
                new Wait(driver).ForDialogPopup(),
                parent,
                report
            );
        }

        //Completed

    }
}