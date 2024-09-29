using System;
using System.Linq;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Menus.SideMenus;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps
{
    public abstract class AppBasePage<TSelf> : BasePage<TSelf>
        where TSelf : AppBasePage<TSelf>
    {
       public AppBasePage(IWebDriver driver, Report report) : base(driver, report)
        { }

        ///<summary>
        /// Helper function to navigate to a link in `Side Menu > Modules` from the dashboard page.
        // These few lines exist at the start of pretty much every single test.
        ///</summary>
        /// <typeparam name="T">The type of the resulting page to navigate to</typeparam>
        /// <param name="module">The module that the link is contained in</param>
        /// <param name="linkText">The link text within the module menu</param>
        /// <returns>The resulting page object of type T</returns>
        public T NavigateTo<T>(string module, string linkText)
            where T : BasePage<T>
        {
            var page = SideMenu()
                .Expand("Modules")
                .SelectModule(module)
                .ClickExpandAll()
                .NavigateTo<T>(linkText);

            // When the environment is slow, the blocking indicator can take seconds to appear
            // When the environment is fast, the blocking indicator can be so fast that the below check fails.
            try
            {
                new Wait(driver, 5).UntilDisplayed(Selectors.Blocking());
            }
            catch (WebDriverTimeoutException) { }
            // NavigateTo is often the first action in a test
            // This can take a very long time in the mornings when dynamics hasn't "warmed up"
            // hence the 120s wait.
            new Wait(driver, 120).ForBlocking();

            return page;
        }

        // TODO: This should be the actual page, not an `AppBasePage`, but that will require a move to the
        // curiously recurring template pattern for AppBasePage.
        public SideMenu<TSelf> SideMenu()
        {
            return new(
                driver.FindElement(By.ClassName("modulesPane")),
                (TSelf)this,
                report
            );
        }

        public TSelf SelectView(string viewText)
        {
            new Wait(driver)
                .Until(d => d
                .FindElements(By.CssSelector("button[name='SystemDefinedManageViewFilters']"))
                .First(e => e.Displayed)
                )
            .Click();

            new Wait(driver)
                .UntilDisplayed(By.CssSelector("[data-dyn-controlname='GlobalGroup']"))
                .FindElements(By.TagName("button"))
                .First(view => view.Text.Equals(viewText))
                .Click();

            new Wait(driver).ForBlocking();

            return (TSelf)this;
        }

        public TSelf SetFilter(string text)
        {
            return SetInput(
                "Filter",
                new Wait(driver).UntilDisplayed(By.CssSelector("input[aria-label='Filter']")),
                text + Keys.Enter
            );
        }

        public TSelf SetFilter(string text, string filterColumn)
        {
            SetInput(
                "Filter",
                new Wait(driver).UntilDisplayed(By.CssSelector("input[aria-label='Filter']")),
                text
            );

            new Wait(driver)
                .UntilClickable(By.XPath($".//ul[@role='listbox']//span[@class='quickFilter-listFieldName' and text()='{filterColumn}']/.."))
                .Click();

            return (TSelf)this;
        }

        /// <summary>
        /// Check if a button is disabled.
        /// This function is intended for use in test assertions, *not* to check whether a button is clickable.
        /// </summary>
        /// <param name="buttonName">The name of the button to check</param>
        /// <returns>Whether the given button is disabled</returns>
        public bool ButtonIsDisabled(string buttonName)
        {
            var button = Root().FindElement(Selectors.Button(buttonName));
            var isDisabled = "true".Equals(button.GetAttribute("disabled"));
            Log().Pass($"Button '<b>{buttonName}</b>' is <b>{(isDisabled ? "" : "not ")}disabled</b>");
            return isDisabled;
        }

        public ActionCentreDialog<TSelf> ClickShowMessages()
        {
            Log().Click("Show messages");
            driver.FindElement(By.CssSelector("button[data-dyn-controlname='navBarMessageCenter']")).Click();

            return new(
            new Wait(driver).Until(d => d.FindElement(By.ClassName("messageCenter"))),
            (TSelf)this,
            report  
            );            
        }

        /// <summary>
        /// A utility to explicitly wait for a "Processing operation" dialog with an activity timer.
        /// </summary>
        /// <returns>The current page object</returns>
        public TSelf WaitForOperationProcessing()
        {
            Log().Info("Wait for 'Processing Operation'");

            new Wait(driver).ForBlocking();

            // Wait for appearance of relevant dialog.
            new Wait(driver, 20).Until(d =>
                d.FindElement(Selectors.OperationProcessing())
            );
            // Wait for disappearance of relevant dialog
            new Wait(driver, 120).Until(d =>
                !d.FindElements(Selectors.OperationProcessing()).Any()
            );
            return (TSelf)this;
        }   
    }
}
//COmpleted