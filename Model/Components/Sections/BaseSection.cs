using System;
using System.Linq;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Sections
{
    public abstract class BaseSection<TParent, TSelf> : BaseComponent<TParent, TSelf>
        where TParent : BaseObject<TParent>
        where TSelf : BaseSection<TParent, TSelf>
    {
        protected readonly string sectionName;

        protected BaseSection(string sectionName, TParent parent, Report report, string? displayName = null)
            : base(GetSectionElement(parent.PublicRoot(), sectionName), parent, report, displayName)
        {
            this.sectionName = sectionName;
            Expand();
        }

        protected static IWebElement GetSectionElement(ISearchContext root, string sectionName)
        {
            return new Wait(((IWrapsDriver)root).WrappedDriver, 5).Until(d => root
                .FindElements(By.CssSelector("[data-dyn-role='SectionPage']"))
                .First(e => e.Displayed && e.FindElements(Selectors.Button(sectionName)).Any())
            );
        }

        protected IWebElement GetSectionHeaderButton()
        {
            return Root().FindElement(Selectors.Button(sectionName));
        }

        public TSelf Expand()
        {
            if (!IsExpanded())
            {
                new Wait(driver).UntilClickable(GetSectionHeaderButton()).Click();
            }

            return (TSelf)this;
        }

        protected bool IsExpanded()
        {
            return GetSectionHeaderButton().GetAttribute("aria-expanded").Equals("true");
        }

        public TParent Collapse()
        {
            if (IsExpanded())
            {
                new Wait(driver).UntilClickable(GetSectionHeaderButton()).Click();
            }

            return parent;
        }

        public TParent Within(Action<TSelf> action)
        {
            action((TSelf)this);
            return Collapse();
        }

        //Completed


    }
}