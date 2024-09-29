using System;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Popups
{
    // Please note: this is a very different dialog to the `AddressSelectionDialog`.
    public class UsageRegistrationPopup<TParent> : BaseSinglePageDialog<TParent, UsageRegistrationPopup<TParent>>
        where TParent : BaseObject<TParent>
    {

        public UsageRegistrationPopup(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
        }

        public UsageRegistrationPopup<TParent> StoreLastUsage(out int lastUsage)
        {
            lastUsage = ConvertUsageToValue(
                Root()
                    .FindElement(By.CssSelector("input[aria-label='Last usage']"))
                    .GetAttribute("title")
            );

            Log().Value("Last usage", lastUsage);

            return this;
        }

        private static int ConvertUsageToValue(string value)
        {
            return string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value.Replace(",", ""));
        }

        public UsageRegistrationPopup<TParent> SetCurrentUsage(int updatedUsage)
        {
            var input = Root().FindElement(By.CssSelector("input[aria-label='Current usage']"));

            SetInput(
                "Current usage",
                input,
                updatedUsage.ToString() + Keys.Tab
            );

            return this;
        }

        public UsageRegistrationPopup<TParent> SetUsageDate(DateTime usageDate)
        {
            var input = Root().FindElement(By.CssSelector("input[aria-label='Usage date']"));

            SetInput(
                "Usage date",
                input,
                usageDate.ToString("d/MM/yyyy")
            );

            return this;
        }

        public UsageRegistrationPopup<TParent> SetOverrideUsage()
        {
            Log().Click("Override checkbox");
            driver.FindElement(By.CssSelector(".checkBox[aria-label='Override usage']")).Click();
            return this;
        }

        //Completed


    }
}