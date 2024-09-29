using System.Linq;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class RentalDevicesPage : BaseCRUDPage<RentalDevicesPage>
    {
        public RentalDevicesPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public RentalDevicesPage FindActiveRentalNotWorking(out string rentalDevice)
        {
            Log().Info("Searching for an Active device with status = not working");

            GetDataTable().SetFilterByText("Location ID", Filter.IsNot, "WORKING");
            // rentalDevice = GetDataTable().GetCellValue("Current status", "Active", "Rental device");

            var device = "";
            SuppressLogs(() =>
            {
                var currentStatusColumn = GetDataTable().GetColumnValues("Current status");
                var rentalDeviceColumn = GetDataTable().GetColumnValues("Rental device");

                var deviceIndex = Enumerable.Range(0, currentStatusColumn.Count)
                    .First(i => currentStatusColumn[i].Equals("Active"));
                device = rentalDeviceColumn[deviceIndex];
            });
            rentalDevice = device;

            Log().DataSelected("Rental device (active, not working)", rentalDevice);
            Log().Info($"Selected device (status Active): {rentalDevice}");

            return this;
        }

        public DataTable<RentalDevicesPage> GetDataTable()
        {
            return new(
                new Wait(driver, 5).UntilDisplayed(Selectors.Table("Rental devices")),
                this,
                report,
                "Rental devices"
            );
        }
        //Completed
    }
}