using OpenQA.Selenium;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Support.Utils.Logging;
using System.Threading;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Model.Data.Enums;
using OpenQA.Selenium.Interactions;
using System;
using System.Linq;

namespace dynamics365accelerator.Model.Components.Popups
{
    // Please note: this is a very different dialog to the `AddressSelectionDialog`.
    public class AddressSelectionPopup<TParent> : BaseComponent<TParent, AddressSelectionPopup<TParent>>
        where TParent : BaseObject<TParent>
    {

        public AddressSelectionPopup(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
        }

        public TParent ClickAddress(AddressData address)
        {
            Log().Info($"Click <b>Address</b>: {address.FormatForLog()}");

            var element = Root().FindElement(By.CssSelector($"[data-dyn-controlname='LogisticsPostalAddress_Street'] [title='{address.Street}']"));

            new Actions(driver)
                .MoveToElement(element)
                .Pause(TimeSpan.FromSeconds(1))
                .Click()
                .Pause(TimeSpan.FromSeconds(1))
                .Perform();

            return parent;
        }

        public TParent SelectWhere(string city)
        {

            GetDataTable().SetFilterByText("City", Filter.IsExactly, city);
            Log().Info($"Select where 'City' is '{city}'");
            Thread.Sleep(1000);
            driver.FindElement(Selectors.Table("Addresses")).SendKeys(Keys.Enter);
            return parent;
        }

        public DataTable<AddressSelectionPopup<TParent>> GetDataTable()
        {
            return new(
                new Wait(driver, 5).UntilDisplayed(Selectors.Table("Addresses")),
                this,
                report,
                "Addresses"
            );
        }

        public AddressData GetSelectedAddress()
        {
            var row = GetDataTable().GetSelectedRow();
            var address = new AddressData(
                Name: row.GetCellValue("Name or description"),
                Street: row.GetCellValue("Street"),
                City: row.GetCellValue("City"),
                PostCode: row.GetCellValue("ZIP/postcode")
            );
            return Log().Value("Selected address", address);
        }

        public TParent Close()
        {
            Log().Info("Close popup");
            // This is an element that, when clicked, should trigger the popup closing.
            // Not guaranteed to work on all pages.
            driver.FindElement(By.CssSelector("[data-dyn-controlname='LineView_header']")).Click();
            return parent;
        }

        //Completed
    }
}