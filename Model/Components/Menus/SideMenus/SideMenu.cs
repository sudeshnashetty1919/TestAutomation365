using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using dynamics365accelerator.Model.Pages.FinOps;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Support.Utils;
using System.Linq;

namespace dynamics365accelerator.Model.Components.Menus.SideMenus
{
    public class SideMenu<TParent> : BaseComponent<TParent, SideMenu<TParent>>
        where TParent : BaseObject<TParent>
    {
        public SideMenu(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
            new Wait(driver, 60).UntilNotDisplayed(By.CssSelector("[data-dyn-controlname='DialogGroup']"));
            new Wait(driver).UntilDisplayed(By.ClassName("modulesList"));
        }

        public DashboardPage ClickHome()
        {
            Log().Click("Home");

            Root().FindElement(Selectors.Title("Home")).Click();

            return new DashboardPage(driver, report);
        }

        public SideMenu<TParent> Expand(string section)
        {
            Log().Info($"Expand '{section}'");
            By selector = By.CssSelector($".modulesPane-groupHeading[aria-label='{section}']");

            new Wait(driver, 10).Until(d =>
            {
                var e = d.FindElement(selector);

                if ("false".Equals(e.GetAttribute("aria-expanded"))) e.Click();

                return new Wait(driver, 1).Until(d => "true".Equals(d.FindElement(selector).GetAttribute("aria-expanded")));
            });

            return this;
        }

        public ModulesFlyout<TParent> SelectModule(string module)
        {
            Log().Click($"{module} module");

            new Wait(driver).ForBlocking();

            // When the system isn't "warmed up" the Modules dropdown can take quite a while to load
            new Wait(driver, 60).Until(d => !Root()
                .FindElements(By.ClassName("modulesFlyout-isLoading"))
                .Where(e => e.Displayed)
                .Any());

            new Wait(driver, 10).Retry(d => Root().FindElement(Selectors.Title(module)).Click());

            return new(Root(), parent, report);
        }
        //Completed

    }
}

