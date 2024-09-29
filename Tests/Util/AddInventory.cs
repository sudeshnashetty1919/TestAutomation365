using System.Collections.Generic;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Model.Pages.Finops;
using dynamics365accelerator.Model.Pages.Finops.InventoryManagement;
using NUnit.Framework;
//dynamics365accelerator -- dynamics365accelerator
namespace dynamics365accelerator.Tests.Util
{
    public class AddInventoryUtil : BaseTests
    {
        //<summary>
        //Utility to add inventory to a warehouse
        //Set the below variables to:
        // - the warehouse to add stock to
        // - the items to add
        // - how much quantity to add
        //<summary>

        [Test]
        [Ignore("To be run when neccesaary")]

        public void AddInventory()
        {
            var warehouse = "29C";
            var items = new List<string>{
                "24310",
                "1078",
                "5050bBnZ"

            };
            var quantity = 10000; 

            var page = Open<DashboardPage>()
                .NavigateTo<InventoryAdjustmentPage>("Inventory management","Inventory adjustment")
                .ClickNewInventoryAdjustment()
                .SetWarehouse(warehouse)
                .ClickOk();

            items.ForEach(item =>
            {
                page
                    .ClickNewJournalLine()
                    .SetItemNumber(item)
                    //.SetLocation(warehouse)
                    .SetQuantity(quantity);
            });

            page
                .ClickPost()
                .ClickOk()
                .Assert.ThatOperationMessage(Should.Match,$"Journal: {page.GetJournalNumber()} Journal has been posted.");   


        }
    }
}
