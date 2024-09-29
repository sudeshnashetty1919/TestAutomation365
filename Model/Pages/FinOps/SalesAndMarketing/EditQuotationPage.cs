using System;
using OpenQA.Selenium;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Model.Components.Dialogs.SalesAndMarketing;
using dynamics365accelerator.Model.Pages.FinOps.AccountsReceivable;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class EditQuotationPage : BaseEditOrderPage<EditQuotationPage>
    {
        public EditQuotationPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public MaintainChargesPage ClickMaintainCharges()
        {
            TopMenu().ClickCRUDButtonInGroup("Charges", "Maintain charges");
            return new MaintainChargesPage(driver, report);
        }

        public EditQuotationPage SetItem(string itemNumber)
        {
            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetFirstEmptyCell("Item")
                    .FindElement(By.TagName("input"));

                SetInput("Item number", input, itemNumber + Keys.Tab);
            });

            return this;
        }

        public new EditQuotationPage SetCellQuantity(double quantity, string itemNumber)
        {
            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetCell("Item", itemNumber, "Quantity")
                    .FindElement(By.TagName("input"));

                SetInput("Quantity", input, quantity + Keys.Tab);
            });

            return this;
        }

        public EditQuotationPage AddItem(OrderItem item)
        {
            SetItem(item.ItemNumber);
            SetCellQuantity(item.Quantity, item.ItemNumber);
            if (item.Warehouse is not null) throw new NotImplementedException();

            return this;
        }

        public SendQuotationDialog ClickSendQuotation()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Send quotation");
            return new(new Wait(driver, 5).ForDialogPopup(), this, report);
        }

        public QuotationJournalPage ClickQuotationJournal()
        {
            TopMenu().ClickCRUDButtonInGroup("Journals", "Quotation journal");
            return new QuotationJournalPage(driver, report);
        }

        public ConfirmQuotationDialog ClickConfirm()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Confirm");
            return new(
                new Wait(driver, 5).ForDialogPopup(),
                this,
                report
            );
        }

        public override DataTable<EditQuotationPage> GetDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("Quotation lines")),
                this,
                report,
                "Quotation lines"
            );
        }

        public CancelOrLoseQuotationDialog ClickCancel()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Cancel");
            return new(
                new Wait(driver, 5).ForDialogPopup(),
                this,
                report
            );
        }

        public CancelOrLoseQuotationDialog ClickLostQuotation()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Lost quotation");
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public EditSalesOrderPage ClickRelatedInformationSalesOrders()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Sales orders");
            new Wait(driver).ForBlocking();
            var page = new EditSalesOrderPage(driver, report);
            string? salesOrder = null;
            SuppressLogs(() =>
            {
                salesOrder = page.GetOrderNumber();
               
            });
            Log().DataCreated("Sales Order Number (confirmed from quotation)", salesOrder);
           
            return page;
        }

        public string GetDeliveryName(string itemNumber)
        {
            var value = GetDataTable()
                .GetCellValue("Item", itemNumber, "Delivery name");
            Log().Value($"Delivery Name (Item: {itemNumber})", value);
            return value;
        }

        public double GetNetAmount(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item", itemNumber, "Net amount");

            var netAmount = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);

            Log().Value($"Net Amount (Item: {itemNumber})", netAmount);

            return netAmount;
        }

        public EditQuotationPage SetTransactionType(string value)
        {

            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetCell("Transaction type", "", "Transaction type")
                    .FindElement(By.TagName("input"));

                input.SendKeys(value + Keys.Tab);
                Log().Value($"Transaction type set to {value}", value);
            });

            return this;
        }

        public EditQuotationPage SetProjectCategory(string value, string TransactionType)
        {

            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetCell("Transaction type", TransactionType, "Project category")
                    .FindElement(By.TagName("input"));

                input.SendKeys(value + Keys.Tab);
                Log().Value($"Project Category set to {value}", value);
            });

            return this;
        }

        public EditQuotationPage SetQuantity(string value, string TransactionType)
        {

            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetCell("Transaction type", TransactionType, "Quantity")
                    .FindElement(By.TagName("input"));

                input.SendKeys(value + Keys.Tab);
                Log().Value($"Quantity set to {value}", value);
            });

            return this;
        }

        public double GetDiscountAmount(string itemNumber)
        {
            double discount = 0.0;
            // This is a bit of a hack - the discount can take a while to populate, so we wait several seconds for it to not be zero.
            try
            {
                new Wait(driver, 10).Until(d =>
                {
                    var value = GetDataTable()
                        .GetCell("Item number", itemNumber, "Discount")
                        .FindElement(By.TagName("input"))
                        .GetAttribute("value");

                    discount = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
                    return discount != 0.0;
                });
            }
            catch { }

            Log().Value($"Discount (Item: {itemNumber})", discount);

            return discount;
        } 
       //Completed
    }
}        