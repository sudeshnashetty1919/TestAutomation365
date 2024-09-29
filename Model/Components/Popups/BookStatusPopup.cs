using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Popups
{
    public class BookStatusPopup<TParent> : BaseComponent<TParent, BookStatusPopup<TParent>>
        where TParent : BaseObject<TParent>
    {

        public BookStatusPopup(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report)
        {
        }

        public TParent SelectBook(string book)
        {
            Log().Click($"Book: {book}");
            GetDataTable().GetCell("Book", book, "Book").Click();
            return parent;
        }

        public string GetBookStatus(string book)
        {
            var status = GetDataTable().GetCellValue("Book", book, "Status");
            Log().Value($"Status (Book: {book})", status);
            return status;
        }

        public DataTable<BookStatusPopup<TParent>> GetDataTable()
        {
            return new(
                driver.FindElement(Selectors.Table("Fixed asset book")),
                this,
                report,
                "Fixed asset book"
            );
        }

        //Completed
    }
}