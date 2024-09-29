using System.Net.Mime;
using OpenQA.Selenium;
using dynamics365accelerator.Support.Utils.Logging;
using System;
using System.Linq;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Model.Data.Enums;
using System.Collections.Generic;

namespace dynamics365accelerator.Model.Components.Popups
{
    public class LocationSearchPopup<TParent> : BaseComponent<TParent, LocationSearchPopup<TParent>>
        where TParent : BaseObject<TParent>
    {

        public LocationSearchPopup(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
        }

        public LocationSearchPopup<TParent> SetToOnHand()
        {
            Log().Info("Set location search to '<b>On-hand</b>'");
            Root().FindElement(Selectors.Name("switchView")).Click();
            driver.FindElements(By.CssSelector("[role='option']"))
                .First(e => e.Text.Equals("On-hand"))
                .Click();
            return this;
        }

        public LocationSearchPopup<TParent> SetWarehouse(string warehouse)
        {
            GetDataTable().SetFilterByText("Warehouse", Filter.IsExactly, warehouse);
            return this;
        }

        public DataTable<LocationSearchPopup<TParent>> GetDataTable()
        {
            return new(
                Root().FindElement(Selectors.Table("Inventory dimensions")),
                this,
                report,
                "Inventory dimensions"
            );
        }

        public TParent SelectFirstLocation()
        {
            Log().Info($"Selecting first location in list");
            GetDataTable().GetColumn("Total available")
                .First()
                .Click();
            return parent;
        }

        public TParent SelectLocationWithStock(int requiredQuantity)
        {
            Log().Info($"Selecting a location with stock of at least {requiredQuantity}");
            GetDataTable().GetColumn("Total available")
                .First(e =>
                {
                    var value = e.FindElement(By.TagName("input")).GetAttribute("value");
                    int totalAvailable = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
                    return totalAvailable >= requiredQuantity;
                })
                .Click();
            return parent;
        }

        public TParent SelectLocationWithoutStock(int quantity)
        {

            Log().Info($"Selecting a location with stock less than {quantity}");
            GetDataTable().GetColumn("Total available")
                .First(e =>
                {
                    var value = e.FindElement(By.TagName("input")).GetAttribute("value");
                    int totalAvailable = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
                    return totalAvailable < quantity;
                })
                .Click();
            return parent;
        }

        public string GetFromLocationWhereTotalAvailableIsLessThan(int quantity)
        {
            Log().Info($"Selecting a location with stock less than {quantity}");
            var value ="";
            var ele = GetDataTable().GetColumn("Total available")
               .First(e =>
               {
                    value = e.FindElement(By.TagName("input")).GetAttribute("value");
                    int totalAvailable = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
                    return totalAvailable < quantity;

               });
           
            string fromLocation = GetDataTable().GetCellValue("Total available",value,"Location");
            Log().Value($"Total Available for location {fromLocation}", value);
            Log().DataSelected("From location", fromLocation);
            return fromLocation;
        }

        public int GetAvailablePhysical(string fromLocation)
        {
         
            string value = GetDataTable().GetCell("Location", fromLocation, "Available physical")
                                .FindElement(By.TagName("input"))
                                .GetAttribute("value");            
            int availablePhysical = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
            Log().Value($"Available Physical for Location {fromLocation}", value);
            return availablePhysical;
        }

        public TParent SelectLocation(string location)
        {            
            Log().Value($"Location selected ",location);
            GetDataTable().GetCell("Location", location).Click();
            return parent;
        }

        //Completed

    }
}