using System;
using dynamics365accelerator.Model;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils
{
    ///<summary>
    ///class that represents a browser tab, with Fluent interface for opening, taking some actions, and closing the tab.
    ///usage:
    ///```
    ///  Open<Tab>()
    ///    .Act(() =>
    ///    {
    ///       //Take some browser-based action within the tab    
    ///    } )
    ///    .Close(); //Return to original tab
    ///```
    ///</summary>

    public class Tab : BaseObject<Tab>
    {
        private readonly string previousTabHandles;
        protected override ISearchContext Root() => driver;

        public Tab (IWebDriver driver, Report report) : base(driver, report)
        {
            previousTabHandles = driver.CurrentWindowHandles;
            Open();
        }

        public Tab Act(Action action)
        {
            action();
            return this;
        }

        private void Open()
        {
            Log().Info("Opening new Tab");
            driver.SwitchTo().NewWindow(WindowType.Tab);
        }

        ///<Summary>
        //Close the tab and return to the tab from wich this tab was created.
        ///</summary>

        public void Close()
        {
            Log().Info("Closing current tab");
            driver.Close();
            driver.SwitchTo().Window(previousTabHandles);
        }
    }

}