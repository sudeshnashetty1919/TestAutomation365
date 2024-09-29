using System;
using System.Linq;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Features
{
    // The financial dimensions fields show up often, are frustrating to handle due to their lack of proper labelling,
    // and due to the different ways they present they are difficult to share code with.
    // Inheritance is a bad fit here due to C#'s lack of multiple inheritance or mixins.
    // The solution is this: an abstract component representing any element with financial dimension fields.
    // It is strongly recommended not to use this directly - construct it internally and redirect it's public functions to new methods.
    // See: `FinancialDimensionsSection`.
    public class FinancialDimensions<TParent>: BaseComponent<TParent, FinancialDimensions<TParent>>
        where TParent: BaseObject<TParent>
    {
        public FinancialDimensions(IWebElement rootElement, TParent parent, Report report) : base(rootElement, parent, report)
        {
        }

        public string GetBusinessUnit() => GetFinancialDimension("BusinessUnit");

        public string GetCostCentre() => GetFinancialDimension("CostCentre");
       
        public TParent SetBusinessUnit(string businessUnit)
        {
            SetInput(
                "Business unit",
                new Wait(driver, 5).Until(d => Root().FindElement(Selectors.BusinessUnit())),
                businessUnit + Keys.Tab
            );
            return parent;
        }

        public TParent SetCostCentre(string costCentre)
        {
            SetInput(
                "Cost centre",
                new Wait(driver, 5).Until(d => Root().FindElement(Selectors.CostCentre())),
                costCentre + Keys.Tab
            );
            return parent;
        }

        protected string GetFinancialDimension(string dimensionName)
        {
            try
            {
                var element = new Wait(driver, 10).Until(d => Root()
                    .FindElements(By.CssSelector($"input[name*='{dimensionName}'][aria-label*='value'],div[name*='{dimensionName}'][aria-label*='value']"))
                    .Where(e => e.Displayed)
                    .First()
                );

                var value = element.TagName.Equals("input", StringComparison.InvariantCultureIgnoreCase)
                    ? element.GetAttribute("value")
                    : element.Text;

                Log().Value(dimensionName, value);
                return value;
            }

            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"No element found to match financial dimension value: {dimensionName}");
            }

        }

        //Completed
       
    }
}
