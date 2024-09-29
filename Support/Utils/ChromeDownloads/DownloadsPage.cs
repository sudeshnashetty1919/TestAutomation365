using System.Collections.Generic;
using System.Linq;
using dynamics365accelerator.Model.Pages;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils.ChromeDownloads
{
    /// <Summary>
    /// <c>DownloadPage</c> models the chrome downloads page, for use in scraping information on download files.
    /// </Summary>

    public class DownloadPage : BasePage<DownloadPage>
    {
        public DownloadPage(IWebDriver driver, Report report) : base(driver, report)
        {
            Log().Info("Navigating to chrome downloads page");
            driver.Navigate().GoToUrl("chrome://downloads");
        }

        public DownloadTile LatesDownload()
        {
            DownloadTile? latestDownload = null;
            new Wait(driver, 10).Retry(d =>
            {
                //It's possible to try open this just as the file is downloaded
                //This wait ensures that we retry at least for a few seconds in case the model is in such a state.
                latestDownload = GetDownloads()[0];
            });
            return latestDownload;
        }

        public List<DownloadTile> GetDownloads()
        {
            return driver
                .FindElement(By.TagName("downloads-manager")).GetShadowRoot()
                .FindElement(By.Id("downloadsList"))
                .FindElements(By.TagName("downloads-item"))
                .Select(e => new DownloadTile(this, e, report))
                .ToList();
        }

        //Completed
    }
}
