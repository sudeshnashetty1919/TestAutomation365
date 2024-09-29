using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class ChargesPage : AppBasePage<ChargesPage>
    {
        public ChargesPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        public ChargesPage SetChargesCode(string chargesCode)
        {
            Log().Info($"Set Charge Code {chargesCode} ");
            var element = GetDataTableForCharges().GetCell("Charges code", "", "Charges code");
            element.Click();
            var input = element.FindElement(By.CssSelector("input[aria-label='Charges code']"));
            input.SendKeys(chargesCode + Keys.Tab);
            return this;
        }

        public ChargesPage SetChargeValue(int chargesValue, string chargesCode)
        {
            var input = GetDataTableForCharges()
                .GetCell("Charges code", chargesCode, "Charges value")
                .FindElement(By.CssSelector("input[aria-label='Charges value']"));
            SetInput(
                $"Charge Value (Code: {chargesCode})",
                input,
                chargesValue.ToString() + Keys.Tab
            );
            return this;
        }

        private DataTable<ChargesPage> GetDataTableForCharges() =>
            new(
                driver.FindElement(Selectors.Table("Charges transactions")),
                this,
                report
            );
    
     //Completed

    }
}