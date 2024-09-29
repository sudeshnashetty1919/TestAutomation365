using System;
using System.Linq;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Dialogs.DeviceManagement;
using dynamics365accelerator.Model.Components.Dialogs.RentalManagement;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Model.Components.Sections.LineDetails;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Model.Pages.FinOps.DeviceManagement;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages.FinOps.RentalManagement
{
    public class EditRentalOrderPage : BaseEditOrderPage<EditRentalOrderPage>
    {
        public EditRentalOrderPage(IWebDriver driver, Report report) : base(driver, report)
        {
        }

        protected override ISearchContext Root()
        {
            return driver.FindElement(Selectors.Page("AMRentTable"));
        }

        public EditRentalOrderPage StoreRentalNumber(out string? rentalNumber)
        {
            var number = GetOrderNumber();
            Log().DataCreated("Rental number", number);
            rentalNumber = number;
            return this;
        }

        public EditRentalOrderPage StoreStartDate(out DateTime startDate)
        {
            var startDateString = GetInputValue("Start date");
            startDate = DateTime.Parse(startDateString);
            return this;
        }

        public EditRentalOrderPage SetCellSalesPrice(string itemNumber, double value)
        {
            var input = GetDataTable()
                .GetCell("Item number", itemNumber, "Sales price")
                .FindElement(By.TagName("input"));
            SetInput($"Sales price (Item: {itemNumber})", input, value.ToString() + Keys.Tab);
            return this;
        }

        public CreateTransferDialog ClickCreateTransfer()
        {
            TopMenu().ClickCRUDButtonInGroup("Create", "Transfer");

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                new EditRentalTransferPage(driver, report),
                report
            );
        }

        public EditDevicePage ClickRentalDevice()
        {
            Log().Click("Rental device");
            driver.FindElement(Selectors.UneditableField("Rental device")).Click();

            return new(driver, report);
        }

        public RentalBrowserDialog<EditRentalOrderPage> ClickProcessReplace()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Replace");

            return new(
                new Wait(driver).ForDialogPopup(),
                this,
                report
            );
        }

        public EditRentalOrderPage StoreDeviceNumber(out string device, string? description = null)
        {
            device = GetInputValue("Rental device");
            Log().DataSelected($"Device number{(description is null ? "" : $" ({description})")}", device);
            return this;
        }

        public RentalDeviceLinePage ClickRentalDeviceLines()
        {
            TopMenu().ClickCRUDButton("Rental device lines");

            return new(driver, report);
        }

        public string GetRentalDeviceNumber()
        {
            var rentalDeviceNumber = "";
            SuppressLogs(() =>
            {
                rentalDeviceNumber = GetOrderNumber();
            });
            Log().Value("Rental device number", rentalDeviceNumber);
            Log().DataSource("Rental device number", rentalDeviceNumber);
            return rentalDeviceNumber;
        }

        public EditRentalOrderPage SetNotes(string notes)
        {
            Log().Set("Notes", notes);

            driver.FindElement(By.CssSelector("textarea[name='Note_Note']")).SendKeys(notes + Keys.Tab);

            return this;
        }

        public GenericDialog<EditRentalOrderPage> ClickProcessConfirmation()
        {
            Log().Click($"Process > Confirmation");

            // There may be two identical buttons called "Confirmation"
            // One is disabled, but it's difficult to discern the two - checking the "disabled" attribute is not reliable.
            // Current solution is to try clicking all of them and see if the dialog appears.
            var dialogRoot = new Wait(driver, 20).Until(d =>
            {
                var elements = Root().FindElements(Selectors.TopMenuGroup("Process"))
                    .Where(group => group.Displayed)
                    .First()
                    .FindElements(Selectors.Button("Confirmation"));

                elements.ToList().ForEach(e =>
                {
                    try
                    {
                        e.Click();
                    }
                    catch { }
                });

                return new Wait(driver, 5).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("Post") && e.Text.Contains("confirmation")));
            });

            return new(
                dialogRoot,
                this,
                report,
                "Post and confirm"
            );
        }

        public GenericDialog<EditRentalOrderPage> ClickProcessRental()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Rental");

            return new(
                new Wait(driver).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("Posting rental"))),
                this,
                report,
                "Process rental"
            );
        }

        public EditRentalOrderPage SelectLine(string itemNumber)
        {
            GetDataTable().SelectRow("Item number", itemNumber);

            return this;
        }

        public RentalJournalsPage ClickAllJournals()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Journals");

            Log().Click("Journals > All journals");
            new Wait(driver, 5).Retry(d => d
                .FindElements(By.CssSelector("[data-dyn-role='MenuItem']"))
                .First(e => e.Text.Equals("All journals"))
                .Click()
            );

            return new(driver, report);
        }

        public PeriodicInvoicingDialog<EditRentalOrderPage> ClickCreateInvoicingJournal()
        {
            TopMenu().ClickCRUDButtonInGroup("Periodic invoicing", "Create invoicing journal");

            return new(
                new Wait(driver).Until(d => d
                    .FindElements(Selectors.Dialog())
                    .First(e => e.Displayed && e.Text.Contains("Periodic invoicing"))),
                this,
                report
            );
        }

        public GenericDialog<EditRentalOrderPage> ClickQuotation()
        {
            TopMenu().ClickCRUDButtonInGroup("Quotation", "Quotation");

            return new(
                new Wait(driver).Until(d => d.FindElements(Selectors.Dialog()).First(e => e.Displayed && e.Text.Contains("Posting quotation"))),
                this,
                report,
                "Posting quotation dialog"
            );
        }

        public EditRentalOrderPage StoreCreatedJournalNumber(out string journalNumber)
        {
            journalNumber = GetMessage(Should.Contain, "has been created")
                .Text
                .Split(" ")
                .First(word => word.StartsWith("CJ"));
            Log().DataCreated("Rental journal", journalNumber);
            return this;
        }

        public RentalInvoicingJournalPage ClickPeriodicInvoicingJournal()
        {
            TopMenu().ClickCRUDButtonInGroup("Journals", "Periodic invoicing journal");

            return new(driver, report);
        }

        public RentalJournalsPage ClickRelatedInformationJournalQuotation()
        {
            TopMenu().ClickCRUDButtonInGroup("Related information", "Journals");

            Log().Click("Journals > Quotation");
            new Wait(driver, 5).Retry(d => d
                .FindElements(By.CssSelector("[data-dyn-role='MenuItem']"))
                .First(e => e.Text.Equals("Quotation"))
                .Click()
            );

            return new(driver, report);
        }

        public double GetNetAmount(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Net amount");
            var netAmount = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Net amount (Item {itemNumber})", netAmount);
            return netAmount;
        }

        public double GetQuantity(string itemNumber)
        {
            var value = GetDataTable().GetCellValue("Item number", itemNumber, "Quantity");
            var quantity = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            Log().Value($"Quantity (Item {itemNumber})", quantity);
            return quantity;
        }

        public InvoicingTransactionsPage ClickInvoicingTransactions()
        {
            TopMenu().ClickCRUDButtonInGroup("Periodic invoicing", "Invoicing transactions");

            return new(driver, report);
        }

        public GenericDialog<EditRentalOrderPage> ClickProcessCollectionRequest()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Collection request");

            return new(
                new Wait(driver).ForDialogPopup("Posting collection request"),
                this,
                report,
                "Posting collection request"
            );
        }

        public GenericDialog<EditRentalOrderPage> ClickProcessReturn()
        {
            TopMenu().ClickCRUDButtonInGroup("Process", "Return");

            return new(
                new Wait(driver).ForDialogPopup("Posting return"),
                this,
                report,
                "Posting return"
            );
        }

        public string GetScheduledReturnDate(string itemNumber)
        {
            var scheduledReturnDate = GetDataTable().GetCellValue("Item number", itemNumber, "Scheduled return date");
            Log().Value($"Scheduled return date (Item: {itemNumber})", scheduledReturnDate);
            return scheduledReturnDate;
        }

        public string? GetItemRentalDevice(string itemNumber)
        {
            string? device = null;

            // Need to wait for the field to propagate changes
            try
            {
                new Wait(driver, 10).Until(d =>
                {
                    device = GetDataTable().GetCellValue("Item number", itemNumber, "Rental device");
                    return !string.IsNullOrEmpty(device);
                });
            }
            catch (WebDriverTimeoutException) { }

            Log().Value($"Device (Item: {itemNumber})", device);

            return device;
        }

        public FinancialDimensionsSection<EditRentalOrderPage> GetFinancialDimensionsSection() => new(this, report);
        public LineDetailsSection<EditRentalOrderPage> GetLineDetailsSection() => new(this, report);

        public LinesSection<EditRentalOrderPage> GetLinesSection() => new("Rental order lines", this, report);
        //Completed

    }
}