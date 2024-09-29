using System;
using System.Linq;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class CashRegisterJournalPage : BaseCRUDPage<CashRegisterJournalPage>
    {
        public CashRegisterJournalPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("DXC_CustCashPaymentEntryInvoice"));
        }

        public CashRegisterJournalPage SetPaymentReference(string paymentReference) => SetInput("Payment reference", paymentReference + Keys.Enter);
        public CashRegisterJournalPage SetMethodOfPayment(string paymentMethod) => SetInput("Method of payment", paymentMethod + Keys.Enter);

        public CashRegisterJournalPage SetAmountTendered(double amount)
        {
            double change = 0.0;
            SuppressLogs(() =>
            {
                change = GetChange();
            });

            SetInput("Amount tendered", amount.ToString() + Keys.Enter);

            SuppressLogs(() =>
            {
                new Wait(driver).Until(d => change != GetChange());
            });

            return this;
        }

        public double GetAmountRoundedDown() => GetInputNumericValue("Amount rounded down");

        public double GetChange() => GetInputNumericValue("Change");

        public GenericDialog<CashRegisterJournalPage> ClickPostTaxInvoice()
        {
            Log().Click("Tax Invoice");
         
            new Wait(driver).Retry(d => Root().FindElement(Selectors.Button("Tax Invoice")).Click());

            // Deal with the "Must have name & phone number" dialog that may appear here.
            try
            {
                new GenericDialog<CashRegisterJournalPage>(
                    new Wait(driver, 5).Until(d => d
                        .FindElements(Selectors.Lightbox())
                        .First(e => e.Text.Contains("Customer Reference"))),
                    this,
                    report,
                    "Customer Reference: Must have name & phone number"
                ).ClickClose();
            }
            catch (WebDriverTimeoutException)
            { }

            return new(
                new Wait(driver).ForLightbox(),
                this,
                report,
                "Warning: Post and print to screen only"
            );
        }

        //Completed
    }
}