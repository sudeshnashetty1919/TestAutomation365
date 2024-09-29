using System;
using System.Collections.Generic;
using System.Linq;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class RentalDeviceLinePage : BaseCRUDPage<RentalDeviceLinePage>
    {
        public RentalDeviceLinePage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("AMRentDeviceLine"));
        }

        public List<string> GetAllDevices()
        {
            return GetDataTable().GetColumnValues("Rental device");
        }

        public string GetRentalStatus(string deviceNumber)
        {
            var status = GetDataTable().GetCellValue("Rental device", deviceNumber, "Rental status");
            Log().Value($"Rental status (Device {deviceNumber})", status);
            return status;
        }

        public string GetDeviceClass(string deviceNumber)
        {
            var _class = GetDataTable().GetCellValue("Rental device", deviceNumber, "Class");
            Log().Value($"Class (Device {deviceNumber})", _class);
            return _class;
        }

        public string GetPickupLocation()
        {
            // Assumes single-line table
            var location = GetDataTable().GetColumnValues("Pickup location").Single();
            Log().Value("Pickup location", location);
            return location;
        }

        public string GetReturnLocation()
        {
            // Assumes single-line table
            var location = GetDataTable().GetColumnValues("Return location").Single();
            Log().Value("Return location", location);
            return location;
        }

        public string GetResponsible()
        {
            // Assumes single-line table
            var responsible = GetDataTable().GetColumnValues("Responsible").Single();
            Log().Value("Responsible", responsible);
            return responsible;
        }

        public string GetPickupDate()
        {
            // Assumes single-line table
            var pickupDate = GetDataTable().GetColumnValues("Pickup date").Single();
            Log().Value("Pickup date", pickupDate);
            return pickupDate;
        }

        public string GetScheduledReturnDate()
        {
            // Assumes single-line table
            var scheduledReturnDate = GetDataTable().GetColumnValues("Scheduled return date").Single();
            Log().Value("Scheduled return date", scheduledReturnDate);
            return scheduledReturnDate;
        }

        public RentalDeviceLinePage SetPickupDate(DateTime pickupDate)
        {
            // Assumes single-line table
            SetInput(
                "Pickup date",
                GetDataTable().GetColumn("Pickup date").Single().FindElement(By.TagName("input")),
                pickupDate.ToString("d/M/yyyy") + Keys.Tab
            );

            try
            {
                return new GenericDialog<RentalDeviceLinePage>(
                    new Wait(driver, 5).ForLightbox(),
                    this,
                    report,
                    "Do you want to update the invoicing start date with the pickup date?"
                ).ClickYes();
            }
            catch (WebDriverTimeoutException)
            {
                return this;
            }
        }

        public RentalDeviceLinePage SetScheduledReturnDate(DateTime endDate)
        {
            // Assumes single-line table
            SetInput(
                "Scheduled return date",
                GetDataTable().GetColumn("Scheduled return date").Single().FindElement(By.TagName("input")),
                endDate.ToString("d/M/yyyy") + Keys.Tab
            );

            return this;
        }

        public DataTable<RentalDeviceLinePage> GetDataTable()
        {
            var table = new DataTable<RentalDeviceLinePage>(
                new Wait(driver, 5).Until(d => d.FindElement(Selectors.Table("Rental device lines"))),
                this,
                report,
                "Rental device lines"
            );

            return table;
        }

        public GenericDialog<RentalDeviceLinePage> ClickCrossRentalActivate()
        {
            TopMenu().ClickCRUDButtonInGroup("Cross rental", "Activate");

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report,
                "Posting activation"
            );
        }

        public RentalDeviceLinePage SetReturnLocation(string location)
        {
            // Assumes single-line table
            SetInput(
                "Return Location",
                GetDataTable().GetColumn("Return location").Single().FindElement(By.TagName("input")),
                location + Keys.Tab
            );

            return this;
        }
        //Completed

    }
}