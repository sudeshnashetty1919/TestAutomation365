using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Model.Components.Table;
using static OpenQA.Selenium.Interactions.WheelInputDevice;
using dynamics365accelerator.Model.Data;

namespace dynamics365accelerator.Model.Components
{
    public class DataTable<TParent> : BaseComponent<TParent, DataTable<TParent>>
      where TParent : BaseObject<TParent>
    {
        public DataTable(IWebElement rootElement, TParent parent, Report report, string? displayName = null)
           : base(rootElement, parent, report, displayName)
        {
            new Wait(driver, 30).Until(d => rootElement.Displayed);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Fundamental operations

        public List<Row<TParent>> GetRows()
        {
            var rowElements = GetDataRowElements();
            return Enumerable.Range(0, rowElements.Count)
                .Select(i => new Row<TParent>(rowElements.ElementAt(i), this, report, $"Row #{i}"))
                .ToList();
            }

        public Row<TParent> GetRow(int rowIndex)
            => rowIndex >= 0 ? GetRows().ElementAt(rowIndex) : GetRows().ElementAt(GetRowCount() + rowIndex);

        public Row<TParent> GetRow(string searchColumn, string searchValue)
            => GetRows().First(row => row.GetCellValue(searchColumn, log: false).Equals(searchValue));

        public Row<TParent> GetRow(int searchColumnIndex, string searchValue)
            => GetRows().First(row => row.GetCellValue(searchColumnIndex).Equals(searchValue));

        public IWebElement GetCell(string searchColumn, string searchValue, string? returnColumn = null)
           => GetRow(searchColumn, searchValue).GetCellElement(returnColumn ?? searchColumn);

        public IWebElement GetCell(string searchColumn, string searchValue, int returnColumnIndex)
           => GetRow(searchColumn, searchValue).GetCellElement(returnColumnIndex);

        public IWebElement GetCell(int searchColumnIndex, string searchValue, int returnColumnIndex)
           => GetRow(searchColumnIndex, searchValue).GetCellElement(returnColumnIndex);

        public IWebElement GetCellInput(string searchColumn, string searchValue, string returnColumn)
          => GetCell(searchColumn, searchValue, returnColumn).FindElement(By.CssSelector("input"));

        public string GetCellValue(string searchColumn, string searchValue, string returnColumn, string attribute = "value")
        {
            string value = GetCellInput(searchColumn, searchValue, returnColumn).GetAttribute(attribute);

            Log().Info($"Find '<b>{returnColumn}</b>' when '<b>{searchColumn}</b>' equals '<b>{searchValue}</b>' => '<b>{value}</b>'");

            return value;
        }

        public TParent SetCellValue<T>(string searchColumn, string searchValue, string setColumn, T setValue)
           => SetInput(
                $"{setColumn} ({searchColumn}: {searchValue})",
                GetCellInput(searchColumn, searchValue, setColumn),
                setValue.ToString() + Keys.Tab
            ).Parent;

        // ------------------------------------------------------------------------------------------------------------
        // Overloads of fundamental operations using table-data interface

        public Row<TParent> GetRow(ICell search) => GetRow(search.ColumnName(), search.Value());
        public IWebElement GetCell(ICell search, string returnColumn) => GetCell(search.ColumnName(), search.Value(), returnColumn);
        public IWebElement GetCellInput(ICell search, string returnColumn) => GetCellInput(search.ColumnName(), search.Value(), returnColumn);
        public string GetCellValue(ICell search, string returnColumn) => GetCellValue(search.ColumnName(), search.Value(), returnColumn);
        public TParent SetCellValue(ICell search, ICell set) => SetCellValue(search.ColumnName(), search.Value(), set.ColumnName(), set.Value());

        // ------------------------------------------------------------------------------------------------------------
        // Derived helper functions

        public TParent PopulateLine(Line line)
        {
            var key = line.Values[0];
            SetFirstEmptyCell(key.ColumnName(), key.Value());
            var row = GetRow(key);
            line.Values.GetRange(1, line.Values.Count - 1)
                .ForEach(cell =>
                {
                    row.SetCellValue(cell.ColumnName(), cell.Value());
                });
                return parent;
        }

        public IWebElement GetFirstEmptyCell(string searchColumn) => GetCell(searchColumn, "", searchColumn);

        public TParent SetFirstEmptyCell<T>(string columnName, T value) => SetCellValue(columnName, "", columnName, value);

        public Row<TParent> BeginLineWith<T>(string columnName, T value)
        {
           var row = GetRow(columnName, "");

           row.SetCellValue(columnName, value);

           return row;
        }

        [Obsolete("Replace with Get/SetFirstEmptyCell")]
        public IWebElement GetLastInputCellInColumn(string searchColumn) => GetDataRowElements()
            .Last()
            .FindElements(By.CssSelector("[role='gridcell']"))
            .ElementAt(GetColumnIndex(searchColumn))
            .FindElement(By.CssSelector("input"));

        public TParent ForEachLine(Action<TParent, Row<TParent>> action)
        {
            GetRows().ForEach(row => action(parent, row));
            return parent;
        }

        public TParent ForEachLine(Action<TParent, Row<TParent>> action, Func<Row<TParent>, bool> where)
        {
            GetRows().Where(where).ToList().ForEach(row => action(parent, row));
            return parent;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Column operations

        public List<IWebElement> GetColumn(string searchColumn)
            => GetRows()
                .Select(row => row
                   .GetCellElement(searchColumn)
                ).ToList();

       public List<string> GetColumnValues(string column)
       {
            new Wait(driver).Until(d => GetRowCount() > 0);

            var cellValues = GetColumn(column)
                .Select(cell => cell.FindElement(By.TagName("input")).GetAttribute("value"))
                .ToList();

            Log().Value($"Column {column}", cellValues);

            return cellValues;
        }

        public TParent SetColumnValues(string column, string newValue)
        {
            Log().Info($"Set every cell for {column} in table with new value: {newValue}");

            new Wait(driver).Until(d => GetRowCount() > 0);

            GetColumn(column)
                .Select(e => e.FindElement(By.TagName("input")))
                .Where(current => current.GetAttribute("value") != newValue)
                .ToList()
                .ForEach(input => SetInput(column, input, newValue + Keys.Tab));

            return parent;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Row select handlers
        public Row<TParent> GetSelectedRow()
        {
            var rowElements = GetDataRowElements();
            return Enumerable.Range(0, rowElements.Count)
                .Where(i => "true".Equals(rowElements.ElementAt(i).GetAttribute("data-dyn-row-active")))
                .Select(i => new Row<TParent>(rowElements.ElementAt(i), this, report, "Selected Row"))
                .Single();
        }

        public TParent SelectRow(string searchColumn, string searchValue) => GetRow(searchColumn, searchValue).Select().Parent.Parent;

        public TParent UnselectRow(string searchColumn, string searchValue) => GetRow(searchColumn, searchValue).Unselect().Parent.Parent;

        private IWebElement GetSelectAllCheckbox() => Root().FindElement(By.CssSelector("[role='checkbox'][title='Select or unselect all rows']"));

        public TParent UnselectAll()
        {
            Log().Click("Unselect all lines");

            var checkbox = GetSelectAllCheckbox();

            // If the checkbox is not checked, zero or more lines might be checked.
            // click it once to select everything...
            if ("false".Equals(checkbox.GetAttribute("aria-checked"))) checkbox.FindElement(By.TagName("svg")).Click();

            // ...then click it again to ensure nothing is selected.
            checkbox.FindElement(By.TagName("svg")).Click();

            return parent;
        }

        public TParent SelectAll()
        {
            Log().Click("Select all lines");

            var checkbox = GetSelectAllCheckbox();

            if ("false".Equals(checkbox.GetAttribute("aria-checked"))) checkbox.FindElement(By.TagName("svg")).Click();

            return parent;
        }  
        
         // ------------------------------------------------------------------------------------------------------------
        // Filter helpers

        public ColumnFilter<DataTable<TParent>> GetFilter(string title)
        {
            new Wait(driver, 2).Retry(d => Root()
                .FindElements(By.CssSelector(".dyn-headerCellLabel"))
                .First(e => e.Text.Equals(title, StringComparison.CurrentCultureIgnoreCase))
                .Click());

            return new(
                title,
                new Wait(driver, 10).Until(d => d.FindElements(By.ClassName("columnHeader-popup")).First(e => e.Displayed)),
                this,
                report
                );
        }

        public TParent SetFilterByText(string title, Filter filterMethod, string filterText, bool waitForRowChange = true, int waitTime = 5)
        {
            Log().Info($"Set table filter <b>'{title}'</b> to <b>{filterMethod.AsString()} '{filterText}'</b>");

            var rows = GetRowCount();
            WaitForRowChange(() =>
            {
                GetFilter(title)
                .SetFilterMethod(filterMethod)
                .SetFilterText(filterText)
                .ClickApply(); ;
            }, wait: waitForRowChange ? waitTime : 0);

            return parent;
        }

        public TParent ClearFilter(string title, bool waitForRowChange = true)
        {
            Log().Info($"Clear table filter '{title}'");

            WaitForRowChange(() =>
            {
                GetFilter(title).ClickClear();
            }, wait: waitForRowChange ? 5 : 0);

            return parent;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Assorted helper functions

        private IReadOnlyCollection<IWebElement> GetDataRowElements()
        {
            // The rows can change and cause stale element exceptions after a filter change,
            // even with `SetFilterByText`'s wait-for-row-change functionality.
           // A 2-second retry is used to ensure the rows are obtained correctly.
            IReadOnlyCollection<IWebElement>? rows = null;
            new Wait(driver, 2).Retry(d =>
            {
                rows = Root()
                    .FindElements(By.CssSelector("[role='row']"))
                    .Where(e => e.FindElements(By.CssSelector("[role='gridcell']")).Any())
                    .ToList();
            });
            return rows!;
        }

        public int GetRowCount() => GetRows().Count;

        public int GetColumnIndex(string columnName)
        {
            int index = -1;

                        // This wait exists to prevent the column elements from become stale mid-action.
                        // It's intentionally quite short and with a quick sleep interval to try and keep table actions as fast as possible.
            new Wait(driver, 5, 250).Until(d =>
            {
                var headerElements = Root().FindElements(By.CssSelector("[role='columnheader']"));

                                // Can uncomment below line and view in a debugger to check which table is actually being found.
                var headers = headerElements.Select(e => $"{e.Text} :: {e.GetAttribute("innerHTML")}").ToList();

                index = Wait.WithImplicitWait(driver, TimeSpan.FromSeconds(0), d =>
                    Enumerable.Range(0, headerElements.Count)
                        .First(i =>
                        {
                            var element = headerElements.ElementAt(i);
                            return element.Text.Equals(columnName)
                                || element.FindElements(By.CssSelector($"div[title='{columnName}']")).Any()
                                || element.FindElements(By.CssSelector($"div[data-dyn-qtip-title='{columnName}']")).Any();
                        })
                );

                return index >= 0;
            });
            return index;
        }

        /// <summary>
        /// Scroll the table in a direction for a given distance.
        /// Try to avoid using this, it results in flaky and complex automation code, but it is sometimes unavoidable.
        /// Before using `Scroll`, consider the following alternatives:
        ///  - Is D365 set to the smaller-scale UI in settings?
        ///  - If a column is out of view, will a custom view and `SelectView` work instead?
        ///  - If a row is out of view, can a filter be used instead?
        /// </summary>
        public DataTable<TParent> Scroll(Direction direction, int distance)
        {
            IWebElement scrollElement;
            try
            {
                        // Selectors for scroll elements are different depending on horizontal/vertical
                if (direction.IsHorizontal())
                {
                    scrollElement = Root().FindElement(By.CssSelector(".ScrollbarLayout_face.ScrollbarLayout_faceHorizontal.public_Scrollbar_face"));
                }
                else if (direction.IsVertical())
                {
                    scrollElement = Root().FindElement(By.CssSelector(".ScrollbarLayout_face.ScrollbarLayout_faceVertical.public_Scrollbar_face"));
                }
                else
                {
                    throw new InvalidOperationException($"No such direction: {direction}");
                }
            }
            catch (NoSuchElementException)
            {
                 // If the scroll element isn't visible, there is no requirement to scroll (the whole table is horizontally visible), so we return early.
                return this;
            }

            new Actions(driver)
                .ScrollFromOrigin(
                    new ScrollOrigin() { Element = scrollElement },
                    deltaX: distance * (direction switch
                    {
                        Direction.Left => -1,
                        Direction.Right => 1,
                        _=> 0,
                    }),
                    deltaY: distance * (direction switch
                    {
                        Direction.Up => -1,
                        Direction.Down => 1,
                        _ => 0,
                    }))
                .Perform();

            return this;
        }  

        /// <summary>
        /// Helper function to perform an action and wait for the row count to change.
        /// After waiting it does *not* fail with a timeout, it continues the test flow.
        /// This is because most conditions where the row count changes after an action is not guaranteed, i.e. changing a filter may not change the total number of rows.
        /// Waiting can be avoided by setting `wait` to 0.
        /// </summary>
        private TParent WaitForRowChange(Action action, int wait)
        {
            var rows = GetRowCount();

            action();

            try
            {
                new Wait(driver, wait).Until(d => GetRowCount() != rows);
            }
            catch (WebDriverTimeoutException) { }

            return parent;
        }
    }

    public class Row<TParent> : BaseComponent<DataTable<TParent>, Row<TParent>>
        where TParent : BaseObject<TParent>
    {
        public Row(IWebElement rootElement, DataTable<TParent> parent, Report report, string displayName)
            : base(rootElement, parent, report, displayName)
        {
             // ((IJavaScriptExecutor)driver).ExecuteScript($"arguments[0].setAttribute('data-planit', '{displayName}');", rootElement);
        }

        public List<IWebElement> GetCells() => Root().FindElements(By.CssSelector("[role='gridcell']")).ToList();
        public IWebElement GetCellElement(int columnIndex) => GetCells().ElementAt(columnIndex);
        public IWebElement GetCellElement(string columnName) => GetCellElement(parent.GetColumnIndex(columnName));
        public IWebElement GetCellInput(string columnName) => GetCellElement(columnName).FindElement(By.TagName("input"));
        public IWebElement GetCellInput(int columnIndex) => GetCellElement(columnIndex).FindElement(By.TagName("input"));
        public string GetCellValue(string columnName, bool log = true) => log ? GetCellValueLogged(columnName) : GetCellValueNotLogged(columnName);
        public string GetCellValue(int columnIndex) => GetCellInput(columnIndex).GetAttribute("value");

        private string GetCellValueLogged(string columnName) => Log().Value(columnName, GetCellValueNotLogged(columnName));
        private string GetCellValueNotLogged(string columnName)
        {
            try
            {
                return GetCellInput(columnName).GetAttribute("value");
            }
            catch (NoSuchElementException)
            {
                return GetCellElement(columnName).FindElement(By.TagName("textarea")).GetAttribute("title");
            }
        }

        public Row<TParent> SetCellValue<T>(string columnName, T value) => SetInput(
                    $"Row {displayName} - {columnName}",
                    GetCellInput(columnName),
                    value.ToString() + Keys.Tab
                );

        public Row<TParent> Select()
        {
            Log().Click("Select");
            SuppressLogs(() =>
            {
                if (!IsSelected()) GetCellElement(0).FindElement(By.CssSelector("[role='checkbox']")).Click();
            });
            return this;
        }

        public Row<TParent> Unselect()
        {
            Log().Click("Unselect");
            SuppressLogs(() =>
            {
                if (IsSelected()) GetCellElement(0).FindElement(By.CssSelector("[role='checkbox']")).Click();
            });
            return this;
        }

        public bool IsSelected()
        {
            var selected = GetCellElement(0)
                .FindElement(By.CssSelector("[role='checkbox']"))
                .GetAttribute("aria-checked").Equals("true");
            Log().Info($"Line <b>is{(selected ? "" : " not")} selected</b>");
            return selected;
        }
        //Completed

    }
}
