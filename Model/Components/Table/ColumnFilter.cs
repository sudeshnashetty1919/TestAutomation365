using System.Linq;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Table
{
    public class ColumnFilter<TParent> : BaseComponent<TParent, ColumnFilter<TParent>>
        where TParent : BaseObject<TParent>
    {
        private readonly string title;

        public ColumnFilter(string title, IWebElement rootElement, TParent parent, Report report) : base(rootElement, parent, report)
        {
           this.title = title;
        }

        internal override string GetClassName()
        {
            return $"Filter: {title}";
        }

        public ColumnFilter<TParent> SetFilterMethod(Filter filter)
        {
            new Wait(driver, 2).Retry(d => Root().FindElement(By.ClassName("button-label-dropDown")).Click());

            new Wait(driver, 5).Until(d => d
                    .FindElements(By.CssSelector("button[data-dyn-role='MenuItem']"))
                    .Where(e => e.Text.Equals(filter.AsString()))
                    .First() 
                 ).Click();

            return this;
        }

        public ColumnFilter<TParent> SetFilterText(string text)
        {
            SuppressLogs(() => SetInput("Filter", Root().FindElement(By.CssSelector("input.textbox")), text));

            return this;
        }

        public TParent ClickApply()
        {
            Root().FindElement(By.CssSelector("button[id*='Apply']")).Click();
            return parent;
        }

        public TParent ClickClear()
        {
            Root().FindElement(Selectors.Button("Clear")).Click();
            return parent;
        }

    }//Completed
}