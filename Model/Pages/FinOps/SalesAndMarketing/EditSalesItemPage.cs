using System;
using dynamics365accelerator.Model.Pages.FinOps.InventoryManagement;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{   
    public class EditSalesItemPage : BaseCRUDPage<EditSalesItemPage>
    {
        public EditSalesItemPage(IWebDriver driver, Report report) : base(driver, report) { }

        public EditInventoryTransactionsPage ClickTransactions()
        {
            TopMenu().ClickCRUDButtonInGroup("View", "Transactions");
            return new EditInventoryTransactionsPage(driver, report);
        }

        public CostObjectsPage ClickCostObjects()
        {
            TopMenu().ClickCRUDButtonInGroup("Costing", "Cost objects");

            return new(driver, report);
        }

        public WarehouseItemsPage ClickWarehouseItems()
        {
            TopMenu().ClickCRUDButtonInGroup("Warehouse", "Warehouse items");

            return new(driver, report);
        }

        public OnHandPage ClickOnHandInventory()
        {
            TopMenu().ClickCRUDButtonInGroup("View", "On-hand inventory");

            return new(driver, report);
        }

       //Completed

    }
}        