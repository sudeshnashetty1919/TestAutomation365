using System;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class LineQuantityPage : BaseCRUDPage<LineQuantityPage>
    {
        public LineQuantityPage(IWebDriver driver, Report report)
            : base(driver, report)
        {

        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("SalesTableLineQuantity"));
        }

        public double GetDelivered(string itemNumber)
        {
            var value = GetDataTable()
                .GetCell("Item number", itemNumber, "Delivered")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");

            var delivered = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Delivered (Item {itemNumber})", delivered);

            return delivered;
        }

        public double GetInvoiceRemainder(string itemNumber)
        {
            var value = new Wait(driver).Until(d =>
                GetDataTable()
                    .GetCell("Item number", itemNumber, "Invoice remainder")
                    .FindElement(By.TagName("input"))
            ).GetAttribute("value");

            var invoiceRemainder = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Invoice Remainder (Item {itemNumber})", invoiceRemainder);

            return invoiceRemainder;
        }

        public DataTable<LineQuantityPage> GetDataTable()
        {
            return new(
                new Wait(driver, 10).Until(d => Root().FindElement(Selectors.Table("Order lines"))),
                this,
                report,
                "Order lines"
            );
        }

        public double GetDeliverRemainder(string itemNumber)
        {
            var value = GetDataTable()
                .GetCell("Item number", itemNumber, "Deliver remainder")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");

            var deliverRemainder = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Deliver remainder (Item {itemNumber})", deliverRemainder);

            return deliverRemainder;
        }
        //Completed

}
}