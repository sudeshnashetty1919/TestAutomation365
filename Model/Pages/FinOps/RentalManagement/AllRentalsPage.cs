using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class AllRentalPage : BaseCRUDPage<AllRentalPage>
    {
        public AllRentalPage(IWebDriver driver, Report report) : base(driver, report)
        {}

        public EditRentalOrderPage ClickRental(string rentalOrderNumber)
        {
            Log().Click($"Rental order: {rentalOrderNumber}");
            GetDataTable().SetFilterByText("Rental order", Filter.IsExactly, rentalOrderNumber);
            var input = GetDataTable()
                .GetCell("Rental order", rentalOrderNumber, "Rental order")
                .FindElement(By.TagName("input"));
            input.Click();

            return new(driver, report);
        }

        public DataTable<AllRentalsPage> GetDataTable()
        {
            return new(
                new Wait(driver).UntilDisplayed(Selectors.Table("Rentals")),
                this,
                report,
                "Rentals"
            );
        }
        //Completed
    
    }
}