namespace dynamics365accelerator.Model.API.Data
{
    internal static class IsExternalInit {}
}

public record SalesOrderLine ( string dataAreaId,
                               string InventoryLotId,
                               string FormattedDeliveryAddress,
                               string SalesOrderLineStatus,
                               string ProjectCategoryId,
                               string ItemNumber,
                               string DeliveryAddressDescription,
                               string ShippingWarehouseId,
                               string ProjectId,
                               string ProjectLinePropertyId,
                               double SalesPrice,
                               string FulfilmentStatus)
{}                               