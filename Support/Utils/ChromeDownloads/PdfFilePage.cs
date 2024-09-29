using dynamics365accelerator.Model.Pages;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Threading;

namespace dynamics365accelerator.Support.Utils.ChromeDownloads
{
    /// <Summary>
    /// <c>FilePage</c> represents a PDF file that has been opened in the browser
    /// </Summary>

    public class PdfFilePage : BasePage<PdfFilePage>
    {
        private readonly string filePath;

        public PdfFilePage(IWebDriver driver, Report report, string filePath) : base(driver,report)
        {
            this.filePath = filePath;
            if (!filePath.EndsWith(".pdf"))
            {
                throw new InvalidOperationException($"PdfFilePage opened with file: {filePath}, but file is not a PDF");
            }
            driver.Navigate().GoToUrl("file://" + filePath);
            //Chrome's PDF renderer takes a short time to load. There is nothing that can be waited on, so an explicit wait is required.
            Thread.Sleep(3000);
        }

        public string FilePath => filePath;

        public string FileName => Path.GetFileName(filePath);

        //Completed
    }

}    