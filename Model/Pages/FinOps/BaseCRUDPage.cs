using System;
using System.Linq;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Menus;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps
{
    public abstract class BaseCRUDPage<TSelf> : AppBasePage<TSelf>
        where TSelf : BaseCRUDPage<TSelf>
    {
        protected BaseCRUDPage(IWebDriver driver, Report report)
            : base(driver, report) { }

        public string GetOrderNumber()
        {
            var orderNumber = new Wait(driver)
                .Until(d => d.FindElements(Selectors.HeaderTitle()).First(e => e.Displayed))
                .Text
                .Split(" ")[0];

            Log().Value("Order Number", orderNumber);

            return orderNumber;
        }

        public string GetTitle()
        {
            var title = new Wait(driver)
                .UntilDisplayed(Selectors.HeaderTitle())
                .Text;

            Log().Value("Title", title);

            return title;
        }

        // Top menu
        protected virtual TopCRUDMenu<TSelf> TopMenu()
        {
            return new(
                new Wait(driver, 10).Until(d =>
                    Root()
                        .FindElements(By.CssSelector("[role='presentation'].appBar"))
                        .First(e => e.Displayed)
                    // TODO: when all pages have appropriate root elements, can replace above with below
                    // Root().FindElement(By.CssSelector("[role='presentation'].appBar"))
                ),
                (TSelf)this,
                report
            );
        }

        public TSelf ClickSave()
        {
            return TopMenu().ClickSave();
        }

        public TDialog ClickSaveToDialog<TDialog>()
            where TDialog : BaseSinglePageDialog<TSelf, TDialog>
        {
            TopMenu().ClickSave();

            return PageActions.ConstructComponent<TDialog, TSelf>(
                new Wait(driver).ForDialogPopup(),
                (TSelf)this,
                report
            );
        }

        public TSelf ClickEdit()
        {
            return TopMenu().ClickEdit();
        }

        public TSelf ClickRefresh()
        {
            TopMenu().ClickRefresh();
            new Wait(driver, 60).ForBlocking();
            return (TSelf)this;
        }

        // Avoid at all costs - prefer ClickClose<T> over this function where possible.
        // This function, if used in a fluent chain, almost certainly breaks the model by returning `TSelf`.
        // It exists solely for a particularly nasty chain of parent components in `RSB6.01` which is effectively unmodellable.
        // It shouldn't be used and chained.
        // TODO: Make this function return void. This cannot currently be done as a WM01 test relies on this function.

        [Obsolete("Avoid usage - use ClickClose<T> instead. See comment for details.")]
        public TSelf ClickClose()
        {
            TopMenu().ClickClose();

            return (TSelf)this;
        }

        public TResult ClickClose<TResult>()
            where TResult : BaseObject<TResult>
        {
            new Wait(driver).UntilNotDisplayed(Selectors.Blocking());
            TopMenu().ClickClose();
            return PageActions.ConstructPage<TResult>(driver, report);
        }

        public TResult ClickClose<TParent, TResult>()
            where TResult : BaseComponent<TParent, TResult>
            where TParent : BaseObject<TParent>
        {
            new Wait(driver).UntilNotDisplayed(Selectors.Blocking());
            TopMenu().ClickClose();
            return PageActions.ConstructComponent<TResult, TParent>(
                new Wait(driver).ForDialogPopup(),
                PageActions.ConstructPage<TParent>(driver, report),
                report
            );
        }

        public TSelf ClickTab(string tabName)
        {
            new Wait(driver, 10).Retry(d =>
                TopMenu().ClickTab(tabName)
            );

            return (TSelf)this;
        }

        /// <summary>
        /// Generic method to click a button in the top menu, if necessary.
        /// </summary>
        public TSelf ClickTopMenuButton(string buttonName)
        {
            TopMenu().ClickCRUDButton(buttonName);

            return (TSelf)this;
        }

        /// <summary>
        /// A utility to explicitly wait for a "Generating report"
        /// </summary>
        /// <returns>The current page object</returns>
        public TSelf WaitForReportRetrieval()
        {
            Log().Info("Wait for report generation");

            // The report generations sometimes takes a minute, and sometimes disappears instantly (before the initial wait-to-appear works).
            // If it throws an error, attempt to continue with the test.
            try
            {
                // Wait for appearance of relevant dial
                new Wait(driver, 5).Until(d =>
                    d.FindElements(Selectors.Dialog()).First(e => e.Text.Contains("Waiting"))   
                );
            }
            catch
            {
                return (TSelf)this;
            }

            // Wait for disappearance of relevant dialog
            new Wait(driver, 120).Until(d =>
                !d.FindElements(Selectors.Dialog()).Where(e => e.Text.Contains("Waiting")).Any()
            );

            return (TSelf)this;
        }
    //Completed
    }
}