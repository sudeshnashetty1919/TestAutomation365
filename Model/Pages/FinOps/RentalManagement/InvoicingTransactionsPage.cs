using System;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class InvoicingTransactionsPage : BaseCRUDPage<InvoicingTransactionsPage>
    {
        public InvoicingTransactionsPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public string GetInvoicedTo(string itemNumber)
        {
            var invoicedTo = GetDataTable().GetCellValue("Item number", itemNumber, "Invoiced to");
            Log().Value($"Invoiced to (Item: {itemNumber})", invoicedTo);
            return invoicedTo;
        }

        public string GetInvoicedFrom(string itemNumber)
        {
            var invoicedFrom = GetDataTable().GetCellValue("Item number", itemNumber, "Invoiced from");
            Log().Value($"Invoiced from (Item {itemNumber})", invoicedFrom);
            return invoicedFrom;
        }

        public double GetSalesPrice(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Sales price");
            var salesPrice = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Sales price (Item {itemNumber})", salesPrice);
            return salesPrice;
        }

        public double GetQuantity(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Quantity");
            var quantity = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Quantity (Item {itemNumber})", quantity);
            return quantity;
        }

        public string GetCustomerAccount(string itemNumber)
        {
            var customerAccount = GetDataTable().GetCellValue("Item number", itemNumber, "Customer account");
            Log().Value($"Quantity (Item {itemNumber})", customerAccount);
            return customerAccount;
        }

        public DataTable<InvoicingTransactionsPage> GetDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("Invoicing transactions")),
                this,
                report,
                "Invoicing transactions"
            );
        }//Completed

    }
}
