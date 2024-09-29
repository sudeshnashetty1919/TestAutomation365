using dynamics365accelerator.Support.Utils.Logging;

namespace dynamics365accelerator.Model.Components.Sections
{
    public class InventoryDimensionsSection<TParent> : BaseSection<TParent, InventoryDimensionsSection<TParent>>
        where TParent : BaseObject<TParent>
    {
        public InventoryDimensionsSection(TParent parent, Report report)
            : base("Inventory dimensions", parent, report)
        {
        }

        public string GetWarehouse() => GetInputValue("Warehouse");
    }

    //Completed
}