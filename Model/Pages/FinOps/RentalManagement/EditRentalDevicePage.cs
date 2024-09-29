using System;
using System.Linq;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Model.Components.Sections.LineDetails;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class EditRentalDevicePage : BaseEditOrderPage<EditRentalDevicePage>
    {
        public EditRentalDevicePage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("AMRentDevice"));
        }

        public override DataTable<EditRentalDevicePage> GetDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("Rental lines")),
                this,
                report,
                "Rental lines"
            );
        }

        public FinancialDimensionsSection<EditRentalDevicePage> GetFinancialDimensionsSection() => new(this, report);

        //Completed
    }
}
