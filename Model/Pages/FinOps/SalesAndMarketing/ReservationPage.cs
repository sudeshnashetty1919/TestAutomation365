using OpenQA.Selenium;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils.Logging;
using System;
using dynamics365accelerator.Support.Utils;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class ReservationPage : BaseCRUDPage<ReservationPage>
    {
        public ReservationPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public EditSalesOrderPage ClickClose()
        {
            TopMenu().ClickClose();
            new Wait(driver).ForBlocking();
            return new EditSalesOrderPage(driver, report);
        }

        public ReservationPage SetReservation(string itemNumber, int reservationQuantity)
        {
            var input = GetReservationsDataTable()
                .GetCell("Item number", itemNumber, "Reservation")
                .FindElement(By.TagName("input"));
            SetInput(
                $"Reservation Quantity (Item {itemNumber})",
                input,
                reservationQuantity.ToString()
            );
            return this;
        }

        public ReservationPage StoreTotalAvailable(string itemNumber, out int totalAvailable)
        {
            var value = GetReservationsDataTable()
                .GetCellValue("Item number", itemNumber, "Total available");
            totalAvailable = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
            Log().Value($"Total available (Item {itemNumber})", totalAvailable);
            return this;
        }

        private DataTable<ReservationPage> GetReservationsDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("Inventory reservations")),
                this,
                report,
                "Detailed availability by dimension"
            );
        }

        //Completed


    }
}        