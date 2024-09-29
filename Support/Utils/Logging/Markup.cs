using System.Collections.Generic;
using System.Linq;

namespace dynamics365accelerator.Support.Utils.Logging
{
    //<Summary>
    //Helper tools for generating logging details.
    //</Summary>

    public static class MarkupHelper
    {
       //<Summary>
       //Wrapper to support unified logging within all tests
       //</Summary>

        public static string UnorderedList(List<string> items)
        {
            return "<ul>"
            + string.Join("",items.Select(i => i.StartsWith("<li>") ? i : $"<li>{i}</li>"))
            + $"</ul>";
        }

        //Completed
    }
}