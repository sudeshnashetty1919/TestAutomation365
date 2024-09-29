using System.Linq;
using dynamics365accelerator.Model.Components.Dialogs.InventoryManagement;
using dynamics365accelerator.Model.Components.Menus;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps
{
    public class BaseJournalPage<TSelf> : BaseCRUDPage<TSelf>
        where TSelf : BaseJournalPage<TSelf>
    {
        protected BaseJournalPage(IWebDriver driver, Report report)
            : base(driver, report) { }

        protected override TopCRUDMenu<TSelf> TopMenu()
        {
            return new(
                new Wait(driver, 5).Until(d =>
                    Root().FindElements(By.CssSelector(".appBar-toolbar"))
                        .First(e => e.Displayed)
                ),
                (TSelf)this,
                report
            );
        } 

        public PostJournalDialog<TSelf> ClickPost()
        {
            TopMenu().ClickCRUDButton("Post");

            return new(
                new Wait(driver).ForDialogPopup(),
                (TSelf)this,
                report
            );
        }
        //Completed   
    }
}        