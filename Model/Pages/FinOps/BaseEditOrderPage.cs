using System;
using System.Linq;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps
{
    public abstract class BaseEditOrderPage<TSelf> : BaseCRUDPage<TSelf>
        where TSelf : BaseEditOrderPage<TSelf>
    {
        protected BaseEditOrderPage(IWebDriver driver, Report report)
            : base(driver, report)
        {
        }

        // Page

        private TSelf ClickOrderPageTab(string text, string dataDynControlName)
        {
            Log().Click($"Order tab: {text}");

            new Wait(driver, 5).Until(d => Root()
                .FindElement(By.CssSelector($"[data-dyn-controlname='{dataDynControlName}']")))
                .Click();

            new Wait(driver).ForBlocking();

            return (TSelf)this;
        }

        public TSelf ClickOrderHeader() => ClickOrderPageTab("Header","HeaderView_header");

        public TSelf ClickOrderLines() => ClickOrderPageTab("Lines","LineView_header");

        public string GetStatusInTitle()
        {
            string state = driver
                .FindElement(By.CssSelector("input[name='StatusInTitle']"))
                .GetAttribute("title");

            Log().Value("Order Status", state);

            return state;
        }

        public string GetOrderStatus()
        {
            string state = driver
                .FindElement(By.CssSelector("input[name*='Status']"))
                .GetAttribute("title");

            Log().Value("Order Status", state);

            return state;
        }

        public string GetApprovalStatus()
        {
            var status = driver
                .FindElement(By.CssSelector("div[data-dyn-controlname='HeaderInfo'] input[name='ApprovalStatus']"))
                .GetAttribute("title");
            Log().Value("Approval status", status);
            return status;
        }

        // Table

        // Prefer `LinesSection` over usage of this - we can't rely on a single data table per page.
        public virtual DataTable<TSelf> GetDataTable()
        {
            return new(new Wait(
                driver, 10)
                .Until(d => d.FindElements(By.CssSelector("[aria-label][role='grid']")).First(e => e.Displayed)),
                (TSelf)this,
                report
            );
        }

        // Prefer `LinesSection` over usage of this
        public TSelf SetCellItemNumber(string itemNumber)
        {
            new Wait(driver).Retry(d =>
            {
                var input = GetDataTable()
                    .GetCell("Item number", "", "Item number")
                    .FindElement(By.TagName("input"));

                SetInput("Item number", input, itemNumber + Keys.Tab);
            });

            return (TSelf)this;
        }

        // Prefer `LinesSection` over usage of this
        public TSelf ClickAddLine()
        {
            Log().Click("Add Line");

            int rowCount = GetDataTable().GetRowCount();

            new Wait(driver, 30).Until(d =>
            {
                d.FindElement(Selectors.Button("Add line")).Click();

                return new Wait(driver, 5).Until(d => GetDataTable().GetRowCount() > rowCount);
            });

            return (TSelf)this;
        }

         // Prefer `LinesSection` over usage of this
        public TDialog ClickAddLine<TDialog>()
            where TDialog : BaseSinglePageDialog<TSelf, TDialog>
        {
            Log().Click("Add Line");

            new Wait(driver, 10).Retry(d =>
                d.FindElement(Selectors.Button("Add line")).Click()
            );

            return PageActions.ConstructComponent<TDialog, TSelf>(
                new Wait(driver, 5).Until(d => d.FindElement(Selectors.Lightbox())),
                (TSelf)this,
                report
            );
        }

        // Prefer `LineDetailsTab` over usage of this
        public TSelf ClickSectionTab(string sectionName, string tabName)
        {
            // Should use `LineDetailsSection` abstraction instead of this function.
            new Wait(driver, 5).Retry(d =>
            GetSectionElement(sectionName)
                    .FindElement(By.CssSelector($"li[title='{tabName}']"))
                    .Click()
            );
            return (TSelf)this;
        }

        public UpdateOrderLinesDialog<TSelf> ClickSaveAfterModifyingHeader()
        {
            base.ClickSave();

            return new(
                new Wait(driver).ForDialogPopup(),
                (TSelf)this,
                report
            );
        }

        public TSelf ClickSaveMaybeModifyingHeader()
        {
            base.ClickSave();

            try {
                return new UpdateOrderLinesDialog<TSelf>(
                        new Wait(driver).ForDialogPopup(),
                        (TSelf)this,
                        report
                    )
                    .SetAllSwitchesTo(true)
                    .ClickOk();
               
            } catch (WebDriverTimeoutException) {
                return (TSelf)this;
            }
        }

        public GenericDialog<TSelf> ClickSaveTriggeringDialog()
        {
            ClickSave();

            return new(
                new Wait(driver).Until(d => d.FindElements(Selectors.Dialog()).First(e => e.Displayed && e.Text.Contains("Update order lines"))),
                (TSelf)this,
                report,
                "Update order lines"
            );
        }
    }//Completed
}