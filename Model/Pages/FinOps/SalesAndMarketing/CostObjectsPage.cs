using System;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class CostObjectsPage : BaseCRUDPage<CostObjectsPage>
    {
        public CostObjectsPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public double GetAverageUnitCost(string site = "NZ")
        {
            var value = GetDataTable().GetCellValue("Site", site, "Average unit cost");
            var averageUnitCost = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Average unit cost (Site: {site})", averageUnitCost);
            return averageUnitCost;
        }

        public DataTable<CostObjectsPage> GetDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("On-hand inventory")),
                this,
                report,
                "Cost objects - On-hand inventory"
            );
        }

        //Completed
    }
}       