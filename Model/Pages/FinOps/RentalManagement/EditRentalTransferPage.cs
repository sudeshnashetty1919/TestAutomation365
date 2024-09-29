using System;
using System.Linq;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class EditRentalTransferPage : BaseCRUDPage<EditRentalTransferPage>
    {
        public EditRentalTransferPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public GenericDialog<EditRentalTransferPage> ClickProcessConfirmation()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Confirmation");

            return new(
                new Wait(driver).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("confirmation", StringComparison.InvariantCultureIgnoreCase))),
                this,
                report,
                "Post quotation and confirm"
            );
        }

        public GenericDialog<EditRentalTransferPage> ClickProcessDeparture()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Departure");

            return new(
                new Wait(driver).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("departure", StringComparison.InvariantCultureIgnoreCase))),
                this,
                report,
                "Process departure"
            );
        }

        public GenericDialog<EditRentalTransferPage> ClickProcessArrival()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Arrival");

            return new(
                new Wait(driver).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("arrival", StringComparison.InvariantCultureIgnoreCase))),
                this,
                report,
                "Process arrival"
            );
        }
        //Completed
    }
}