using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Dialogs.AccountsReceivable;
using dynamics365accelerator.Model.Components.Dialogs.InventoryManagement;
using dynamics365accelerator.Model.Components.Dialogs.SalesAndMarketing;
using dynamics365accelerator.Model.Components.Popups;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Model.Components.Sections.LineDetails;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Model.Pages.FinOps.AccountsReceivable;
using dynamics365accelerator.Model.Pages.FinOps.ProcurementAndSourcing;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace dynamics365accelerator.Model.Pages.FinOps.SalesAndMarketing
{
    public class EditSalesOrderPage : BaseEditOrderPage<EditSalesOrderPage>
    {
        public EditSalesOrderPage(IWebDriver driver, Report report) : base(driver, report) { }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("SalesTable"));
        }

        public DeliveryNotePostingDialog<EditSalesOrderPage> ClickGeneratePostDeliveryNote()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Post delivery note");


            var dialog = new DeliveryNotePostingDialog<EditSalesOrderPage>(
                new Wait(driver).ForDialogPopup("Delivery note posting"),
                this,
                report
            );
            // A dialog can appear here with the text "Current Quantity value is All. The recommended value is Picked. Do you want to apply the recommended value?"
            // We attempt to deal with that dialog here

            try
            {
                return new GenericDialog<DeliveryNotePostingDialog<EditSalesOrderPage>>(
                    new Wait(driver, 3).ForLightbox(),
                    dialog,
                    report,
                    "Apply Recommended Quantity Value"
                ).ClickNo();
            }
            catch (WebDriverTimeoutException) { }

            return dialog;
        }

        public CashRegisterPaymentDialog ClickCashRegisterPayment()
        {
            TopMenu().ClickCRUDButton("Cash Register Payment");

            return new(
                // Do not change this for a Selectors.Dialog()
                new Wait(driver, 5).Until(d => d.FindElement(By.CssSelector("[data-dyn-form-name='DXC_CashRegisterPaymentTypeDialog']"))),
                this,
                new CashRegisterJournalPage(driver, report),
                report
            );
        }

        public double GetPhysicalReserved(string itemNumber)
        {
            GetDataTable().Scroll(Direction.Left, 1000);
            var value = GetDataTable()
                .GetCell("Item number", itemNumber, "Physical reserved")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");

            var physicalReserved = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value("Physical Reserved", physicalReserved);

            return physicalReserved;
        }

        public ProductInformationDialog<EditSalesOrderPage> ClickItemNumber(string itemNumber)
        {
            Log().Click($"Item Number: {itemNumber}");
            var dialogRoot = new Wait(driver).Until(d =>
            {
                new Wait(driver,10)
                    .Retry(d =>
                        ClickInputLink(GetDataTable().GetCell("Item number", itemNumber, "Item number")
                            .FindElement(By.TagName("input"))
                    ));
                return new Wait(driver, 2).ForDialogPopup();
            });

            return new(
                dialogRoot,
                this,
                report
            );
        }

        public EditSalesOrderPage CloseSection(string section)
        {
            var element = driver.FindElement(Selectors.Section(section));

            if (element.GetAttribute("aria-expanded").Equals("true"))
            {
                new Wait(driver).Until(ExpectedConditions.ElementToBeClickable(element)).Click();
            }
            return this;
        }

        public string GetBomReference(string itemNumber)
        {
            GetDataTable().SetFilterByText("Item number",Filter.IsExactly, itemNumber);
            var reference = GetDataTable()
                .GetCell("Item number", itemNumber, "BOM reference Id")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");
            GetDataTable().ClearFilter("Item number");
            Log().Value($"BOM Reference for item {itemNumber}", reference);

            return reference is null ? "" : reference;
        }

        public PostingInvoiceDialog<EditSalesOrderPage> ClickGenerateTaxInvoice()
        {
            TopMenu().ClickCRUDButtonInGroup("Generate", "Tax Invoice");

            // Handling dialog internally, as it does not show up for every option.
            try
            {
                new GenericDialog<EditSalesOrderPage>(
                    new Wait(driver, 5).Until(d => d.FindElements(Selectors.Lightbox()).First(e => e.Text.Contains("Customer Reference"))),
                    this,
                    report,
                    "Customer Reference Warning"
                ).ClickClose();
            }
            catch (WebDriverTimeoutException)
            {
                // If the dialog does not appear, do nothing.
            }

            try
            {
                new GenericDialog<EditSalesOrderPage>(
                    new Wait(driver, 2).Until(d => d.FindElements(Selectors.Lightbox()).First(e => e.Text.Contains("Current Quantity value is"))),
                    this,
                    report,
                    "Apply Recommended Value"
                ).ClickNo();
            }
            catch (WebDriverTimeoutException)
            { }

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public EditInvoiceSalesOrderPage ClickJournalsInvoice()
        {
            TopMenu().ClickCRUDButtonInGroup("Journals", "Invoice");

            return new(driver, report);
        }

        public string GetLoadId(string warehouse)
        {
            var element = GetDataTable().GetCell("Warehouse", warehouse, "Load");
            var input = element.FindElement(By.CssSelector("input"));
            input.Click();

            var loadId = input.GetAttribute("value");

            Log().Value($"Load ID (Warehouse {warehouse})", loadId);

            return loadId;
        }

        public EditSalesOrderPage ClickLoadsTab() // Not in TopMenu
        {
            driver.FindElement(Selectors.Title("Loads")).Click();

            return this;
        }

        public double GetDiscountAmount(string itemNumber)
        {
            double discount = 0.0;
            // This is a bit of a hack - the discount can take a while to populate, so we wait several seconds for it to not be zero.
            try
            {
                new Wait(driver, 10).Until(d =>
                {
                    var value = GetDataTable()
                        .GetCell("Item number", itemNumber, "Discount")
                        .FindElement(By.TagName("input"))
                        .GetAttribute("value");

                    discount = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
                    return discount != 0.0;
                });
            }
            catch { }

            Log().Value("Discount", discount);

            return discount;
        }

        public PostingInvoiceDialog<EditSalesOrderPage> ClickTaxInvoice()
        {
            Log().Click("Tax invoice");


            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report);
        }

        public EditSalesOrderPage SetItemNumber(string itemNumber)
        {
            var input = GetDataTable()
                .GetFirstEmptyCell("Item number")
                .FindElement(By.TagName("input"));

            SetInput("Item Number (New Line)", input, itemNumber + Keys.Tab);

            return this;
        }

        public EditSalesOrderPage SetQuantity(double quantity, string itemNumber) => GetDataTable().SetCellValue("Item number", itemNumber, "Quantity", quantity);

        /// <summary>
        /// Perform a search for an item. If the search text is an item number, use `SetItemNumber` instead.
        /// </summary>
        public ProductSearchDialog SearchItems(string search)
        {
            Log().Info($"Searching item number field with text: '<b>{search}</b>'");
            SuppressLogs(() => SetItemNumber(search));
            return new(
                new Wait(driver, 5).ForDialogPopup(),
                this,
                report
            );
        }

        public string GetDeliveryAddressName() => GetInputValue("Delivery address");

        public AddressData GetDeliveryAddress()
        {
            AddressData? address = null;
            SuppressLogs(() =>
            {
                var popup = ClickDeliveryAddressDropdown();
                address = popup.GetSelectedAddress();
                popup.Close();
            });
            Log().Value("Current delivery address", address!.FormatForLog());
            return address;
        }

        public AddressSelectionPopup<EditSalesOrderPage> ClickDeliveryAddressDropdown()
        {
            Log().Click("Delivery address");
            driver.FindElement(By.CssSelector("[data-dyn-controlname='CopyOfReferenceGroup'] .lookupButton")).Click();

            return new(
                new Wait(driver, 5).UntilDisplayed(By.CssSelector("[data-dyn-form-name='LogisticsPostalAddressLookup']")),
                this,
                report
            );
        }

        public DeliveryNoteJournalPage ClickJournalDeliveryNote()
        {
            TopMenu().ClickCRUDButtonInGroup("Journals", "Delivery note");
            new Wait(driver).ForBlocking();
            var deliveryNoteJournalPage = new DeliveryNoteJournalPage(driver, report);
            Log().DataCreated("Delivery Notes", deliveryNoteJournalPage.GetDeliveryNoteNumbers());
            return deliveryNoteJournalPage;
        }

        public EditSalesOrderPage SelectLine(string columnName, string columnValue)
        {
            return GetDataTable().SelectRow(columnName, columnValue);
        }

        public ConfirmCancelDialog ClickCancel()
        {
            TopMenu().ClickCRUDButtonInGroup("Maintain", "Cancel");
            return new(
                driver.FindElement(Selectors.Lightbox()),
                this,
                report
            );
        }

        public CreatePurchaseOrderDialog<EditSalesOrderPage, EditSalesOrderPage> ClickNewPurchaseOrder()
        {
            TopMenu().ClickCRUDButtonInGroup("New", "Purchase order");
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                this,
                report
            );
        }

        public CreateDirectDeliveryDialog<EditSalesOrderPage, EditSalesOrderPage> ClickNewDirectDelivery()
        {
            TopMenu().ClickCRUDButtonInGroup("New", "Direct delivery");
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                this,
                report
            );
        }

        public EditPurchaseOrderPage ClickRelatedPurchaseOrder()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Purchase order");
            new Wait(driver).ForBlocking();
            return new EditPurchaseOrderPage(driver, report);
        }

        public PostingPickingListDialog<EditSalesOrderPage> ClickGeneratePickingList()
        {
            TopMenu().ClickCRUDButton("Generate picking list");
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public EditSalesOrderPage ClickActionsReleaseToWarehouse()
        {
            TopMenu().ClickCRUDButtonInGroup("Actions", "Release to warehouse");

            new Wait(driver).ForBlocking();

            return this;
        }

        public EditPickingListRegistrationPage ClickGeneratePickingListRegistration()
        {
            TopMenu().ClickCRUDButton("Picking list registration");
            return new(driver, report);
        }

        public PickingListRegistrationPage ClickJournalPickingList()
        {
            TopMenu().ClickCRUDButtonInGroup("Journals", "Picking list");
            return new(driver, report);
        }

        public MaintainChargesPage ClickMaintainCharges()
        {
            TopMenu().ClickCRUDButton("Maintain charges");

            return new MaintainChargesPage(driver, report);
        }

        public EditSalesOrderPage SetReservation(string reservation)
        {
            SetInput("Reservation", reservation);
            return this;
        }

        public string GetLineDetailsReservationStatus()
        {
            var reservationStatus = GetSectionElement("Line details").FindElement(Selectors.Input("Reservation")).GetAttribute("value");
            Log().Value("Reservation Status (Line Details)", reservationStatus);
            return reservationStatus;
        }

        public string GetHeaderReservationStatus()
        {
            var reservationStatus = GetSectionElement("Setup").FindElement(Selectors.Input("Reservation")).GetAttribute("value");
            Log().Value("Reservation Status (Line Details)", reservationStatus);
            return reservationStatus;
        }

        public SelectCancellationReasonCodeDialog ClickRemove()
        {
            // Note: `Selectors.Button` does not work here due to the presence of a rubbish bin icon.
            driver.FindElement(By.CssSelector("[data-dyn-controlname='DXC_LineDelete']")).Click();
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                new SalesOrderPage(driver, report),
                report
            );
        }

        public List<string> GetAllItemNumbers()
        {
            // TODO: This should probably use the data table
            var itemNumbers = driver
                .FindElements(By.CssSelector("input[aria-label='Item number']"))
                .Select(e => e.GetAttribute("value"))
                .ToList();
            Log().Value("Item Numbers (Column)", itemNumbers);
            return itemNumbers;
        }

        public RelatedOrdersDialog ClickRelatedOrders()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Related orders");
            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public CustomerAccountPage ClickCustomerAccountNumber()
        {
            Log().Click("Customer account number");
            driver.FindElement(By.CssSelector("[data-dyn-controlname='TabHeaderGeneral_CustAccount']")).Click();
            return new CustomerAccountPage(driver, report);
        }

        public LineQuantityPage ClickLineQuantity()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Line quantity");

            return new(driver, report);
        }

        public override DataTable<EditSalesOrderPage> GetDataTable()
        {
            return new(
                new Wait(driver,5).Until(d => Root().FindElement(Selectors.Table("Order lines"))),
                this,
                report,
                "Order Lines"
            );
        }

        public string GetLineStatus(string itemNumber)
        {
            var lineStatus = GetDataTable()
                .GetCell("Item number", itemNumber, "Line status")
                .FindElement(By.TagName("input"))
                .GetAttribute("value");
            Log().Value($"Line Status (Item {itemNumber})", lineStatus);
            return lineStatus;
        }

        public string GetCustomerReference()
        {
            return GetInputValue("Customer reference");
        }

        public double GetQuantity(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Quantity");
            var quantity = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Quantity (Item {itemNumber})", quantity);
            return quantity;
        }

        public double GetNetAmount(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Net amount");

            var netAmount = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);

            Log().Value($"Net Amount (Item: {itemNumber})", netAmount);

            return netAmount;
        }

        /// <summary>
        /// Updating the line registration may open a page, or if the registration is ok will stay on the current page.
        /// This function handles both possibilities, ensuring a product is properly registered.
        /// </summary>
        public EditSalesOrderPage UpdateLineRegistration()
        {
            ExpandFlyout("Update line").Click("Registration");

            try
            {
                var message = GetMessage(Should.Contain, "Kit adjust amount validation for Sales order");
                if (message is not null)
                {
                    Log().Info("Line is already registered correctly");
                    return this;
                }
            }
            catch { }

            return new SalesOrderRegistrationPage(driver, report)
                .ClickAddRegistrationLine()
                .ClickConfirmRegistration()
                .ClickClose<EditSalesOrderPage>();
        }

        public AutoreservationDialog GetAutoreservationDialog()
        {
            // Click a non-interactive element to trigger the dialog.
            try
            {
                driver.FindElement(Selectors.Name("SalesStatus")).Click();
            }
            catch (NoSuchElementException) { }
            catch (ElementNotInteractableException) { }

            return new(
                new Wait(driver, 5).ForDialogPopup(),
                this,
                report
            );
        }

        public EditSalesOrderPage PostDeliveryNote(string quantityType)
        {
            return ClickTab("Pick and pack")
                .ClickGeneratePostDeliveryNote()
                .SetQuantityType(quantityType)
                .SetSwitch("Print delivery note", false)
                .ClickOk()
                .ClickOk()
                .WaitForOperationProcessing();
        }

        public EditSalesOrderPage PostTaxInvoice()
        {
            return ClickTab("Invoice")
                .ClickGenerateTaxInvoice()
                .SetSwitch("Print invoice", false)
                .ClickOk()
                .ClickOk()
                .WaitForOperationProcessing();
        }

        public LineDetailsSection<EditSalesOrderPage> GetLineDetailsSection() => new(this, report);

        public LinesSection<EditSalesOrderPage> GetLinesSection() => new("Sales order lines", this, report);

        public double GetDeliverRemainder(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Deliver remainder");
            var deliverRemainder = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Sales Order - Deliver Remainder (Item Number: {itemNumber})", deliverRemainder);
            return deliverRemainder;
        }

        public double GetUnitPrice(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Unit price");
            var unitPrice = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Unit Price (Item: {itemNumber})", unitPrice);
            return unitPrice;
        }

        public EditSalesOrderPage CloseTopMenuMessage ()
        {
            Root().FindElement(By.CssSelector("button[title='Close ']")).Click();          
           
            return this;
        }
        //Completed

    }
}