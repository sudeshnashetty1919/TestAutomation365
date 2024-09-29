using System;
using System.Threading;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Pages
{
    public abstract class BasePage<TSelf> : BaseObject<TSelf>
        where TSelf : BasePage<TSelf>
    {
        public BasePage(IWebDriver driver, Report report) : base(driver, report) { }

        // TODO: remove this to enforce roots across all pages.
        protected override ISearchContext Root()
        {
            // Uncomment below line to find which pages are missing root elements.
            // throw new NotImplementedException();
            return driver;
        }

        /// <summary>
        /// Take a screenshot and log it inline in the extent report.
        /// 
        /// Will create a new log line in the report with the image and description (rather than attached at the
        /// bottom of the report).
        /// 
        /// Embedding is done via base64-encoded image - overuse of this function may cause the report file to be
        /// slow to load. 
        /// 
        /// If `saveToFile` is provided, also save it as the provided file name.
        /// </summary>
        /// <param name="description">The description given to the logged screenshot</param>
        /// <param name="saveToFile">If provided, save the screenshot to a file with this name</param>
        public TSelf LogScreenshot(string description, string? saveToFile = null)
        {
            // Hack to ensure pages are loaded.
            Thread.Sleep(3000);

            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            if (saveToFile != null)
            {
                screenshot.SaveAsFile(saveToFile);
            }

            Log().Info(
                "<span>" +
                $" <div>Screenshot: {description}</div>" +
                $" <div><img src='data:image/png;base64,{screenshot.AsBase64EncodedString}' width=1000 /></div>" +
                "</span>"
            );

            return (TSelf)this;
        }
    }

    //Completed
}