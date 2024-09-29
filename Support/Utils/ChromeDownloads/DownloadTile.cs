using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils.ChromeDownloads
{
    /// <Summary>
    /// <c>DownloadTile</c> models a single downloaded file title in the chrome downloads page.
    /// </Summary>

    public class DownloadTile : BaseComponent<DownloadPage,DownloadTile>
    {
        public DownloadTile(DownloadPage parent, IWebElement rootElement, Report report) : base(rootElement, parent, report)
        {
            displayName = GetFileName();
        }

        private ISearchContext ShadowRoot => Root().GetShadowRoot();

        public string GetFileName()
        {
            //'.Text' should work here, but returns an empty string unless a very short wait is applied.
            return ShadowRoot.FindElement(By.Id("file-link")).GetAttribute("textContent");
        }

        public string GetLocalPath()
        {
            return ShadowRoot.FindElement(By.Id("show")).GetAttribute("title");
        }

        public string OpenPdf()
        {
            return new PdfFilePage(driver, Report, GetLocalPath());
        }

        //Completed
    }

}    