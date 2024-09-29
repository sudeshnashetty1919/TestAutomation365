using System.Collections.Generic;

namespace dynamics365accelerator.Model.Components.Table
{
   public interface ICell
   {
      public string ColumnName();
       public string Value();
    }

    public interface ILine
    {
        public Line ToTableLine();
    }

    /// <summary>
    /// Data class representing a bag of data to add to a line in a table.
    /// Contains a `Values` list, which is a `List<ICell>`.
    /// 
    /// Usage, using "Line.Entry" helper:
    /// ```
    /// new DataTable<>()
    ///     .AddLine(new Line(new List<ICell>() {
    ///         Line.Entry("Item number", "1234"),
    ///         Line.Entry("Quantity", 10),
    ///     }))
    /// ```
    /// </summary>
    /// <param name="Values">The list of cell values to add to a table line</param>
    public class Line: ILine
    {
       private readonly List<ICell> values;

       public List<ICell> Values => values;

       public Line(List<ICell> values)
       {
           this.values = values;
       }

       public Line(params ICell[] values)
       {
          this.values = new List<ICell>(values);
       }

        public static ICell Entry<T>(string columnName, T value) => new CellImpl<T>(columnName, value);

        public Line ToTableLine() => this;

        protected class CellImpl<T> : ICell
        {
            private readonly T value;
            private readonly string columnName;

           public CellImpl(string columnName, T value)
           {
                this.columnName = columnName;
                this.value = value;
           }
           public string ColumnName() => columnName;

           public string Value() => (value is null ? "" : value.ToString())!;
           }

           //Completed
    }

}