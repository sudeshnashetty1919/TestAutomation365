using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using dynamics365accelerator.Model;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Support.Utils.Logging;
using iTextSharp.text.pdf.parser;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils
{
    ///<Summary>
    //The Modelling here is a bit of hack - it extends BaseOject to get access to logging and assertions,
    //but isn't actually a model class
    ///</summary>

    public class PdfReader<TParent> : BaseObject<PdfReader<TParent>>
       where TParent : BaseObject<TParent>
    {

        private readonly TParent parent;
        private readonly string filePath;
        private string? text = null;
        protected override ISearchContext Root() => driver;

        public PdfReader(TParent parent, string filePath, IWebDriver driver, Report report) : base(driver, report)
        {
            this.parent = parent;
            this.filePath = filePath;
        }

        public TParent ClosePdf()
        {
            return parent;
        }

        public string FilePath => filePath;
        public string FileName => filePath.Replace("\\","/").Split("/").Last();

        public string Find(Regex matcher)
        {
            return matcher.Match(GetText()).Value;
        }

        public bool Contains(string searchText)
        {
            var result = ContainsTmpl(searchText);
            Log().Info(
                result
                    ? $"Found text:'<b>{searchText}</b>'"
                    : $"Failed to find text: '<b>{searchText}</b>'"
            );
            return result;
        }

        public bool ContainsAddress(string heading, AddressData address)
        {
            var result = ContainsTmpl(heading + "\n" + address.Lines);
            Log().Pass(
                result
                    ? $"Found address under heading '<b>{heading}</b>' : {address.FormatForLog()}"
                    : $"Failed to find address under heading '<b>{heading}</b>' : {address.FormatForLog()}\nAddress could be: {FindAddress(heading)}"
            );
            return result;
        }

        public string FindAddress(string heading)
        {
            return string.Join(
                " , ",
                GetText().Split("\n").SkipWhile(t => !t.Equals(heading)).Take(5).ToList()
            );
        }

        ///<summary>
        // Internal implementation for contains
        //This exists to prevent double-logging
        ///</summary>
        ///<param name="searchText">The string to search for</param>
        ///<return>Whether the string was found in the PDF</return>

        protected bool ContainsImpl(string searchText)
        {
            return GetText().Contains(searchText);
        }

        public override List<string> GetSourceNames()
        {
            return new List<string> {$"PdfReader ({FileName})"};
        }

        protected string GetText()
        {
            if (text is null)
            {
                var output = new StringWriter();
                var reader = new iTextSharp.text.pdf.PdfReader(filePath);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                    output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));

                reader.Close();

                //The PDF is filled with special characters instead of spaces
                text = Regex.Replace(output.ToString(), @"\u00A0", "");    
            }
            return text;
        }


    }   
}
