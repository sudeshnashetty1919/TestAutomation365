using System.Linq;
using System.Threading;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class ItemRequirementsPage : BaseCRUDPage<ItemRequirementsPage>
    {
        public ItemRequirementsPage(IWebDriver driver, Report report)
            : base(driver, report) { }

        public DataTable<ItemRequirementsPage> GetDataTable()
        {
            return new(
                driver.FindElement(By.CssSelector("[data-dyn-controlname='SalesLineGrid']")),
                this,
                report,
                "Item requirements"
            );
        }

        public ItemRequirementsPage SetCell(string columnHeader, string value)
        {
            Thread.Sleep(1000);
            var input = GetDataTable().GetLastInputCellInColumn(columnHeader);
            input.SendKeys(value);
            Log().Info($"Set {value} into {columnHeader}");
            return this;

        }

        public string GetCellValueInColumn(string columnHeader)
        {
            Thread.Sleep(1000);
            var element = GetDataTable().GetLastInputCellInColumn(columnHeader).GetAttribute("value").ToString();
            Log().Value("Value in Table", element);
            return element;

        }

        public ItemRequirementsPage ClickNewItem()
        {
            //This sleep is here as sometimes a small pop up at the top of the screen appears stating 'Please wait' but sometimes it doesnt
            Thread.Sleep(3000);
            TopMenu().ClickNew();

            return new ItemRequirementsPage(driver, report);

        } 

        public ItemRequirementsPage ClickSelectAllItems()
        {

            new Wait(driver, 5).Retry(d =>
                d.FindElement(By.CssSelector("[aria-label='Select all rows']")).Click()
            );
            Log().Info("All rows selected");
            return new ItemRequirementsPage(driver, report);
        }

        public DeliveryNoteDialog ClickDeliveryNoteButton()
        {
            TopMenu().ClickCRUDButton("Manage");
            new Wait(driver, 5).Retry(d => d
                .FindElements(By.ClassName("flyoutButton-Button"))
                .First(e => e.Text.Equals("Posting"))
                .Click()
            );

            new Wait(driver, 5).Retry(d => d
                .FindElements(By.ClassName("button-label"))
                .First(e => e.Text.Equals("Delivery note"))
                .Click()
            );

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );

        }

        public ItemRequirementsPage ClickTabButton(string value)
        {
            Thread.Sleep(3000);
            new Wait(driver, 5).Until(d => d.FindElements(By.ClassName("pivot-label")).First(e => e.Text.Equals(value))).Click();
            Log().Click(value);
            return this;
        }

        public string GetCategory()
        {
            Thread.Sleep(2000);
            var element = driver.FindElement(By.Name("Group_ProjCategoryId")).GetAttribute("value").ToString();
            Log().Value("Category", element);
            return element;
        }

        public string GetOperationCompletedMessage()
        {
            var value = new Wait(driver, 5).Until(d => d.FindElements(By.ClassName("messageBar-message")).First()).GetAttribute("title");
            Log().Value("Operation message", value);
            return value;
        }

        public ItemRequirementsPage SetShowAll()
        {
            var input = new Wait(driver, 5).UntilDisplayed(Selectors.Name("CtrlActiveAll"));
            input.Clear();
            input.SendKeys("All");
            Log().Info("Set combobox to All");
            return this;
        }

        public ItemRequirementsPage ClickLastSelectRow()
        {
            new Wait(driver, 5).Until(d => d.FindElements(By.CssSelector("[aria-label='Select the current row']")).Last()).Click();
            return this;
        }

        //Completed
   
    }
}