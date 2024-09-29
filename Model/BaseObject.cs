using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Model.Components.Popups;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.ChromeDownloads;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace dynamics365accelerator.Model
{
    public abstract class BaseObject<TSelf>
        where TSelf : BaseObject<TSelf>
    {
        protected IWebDriver driver;

        protected Report report;

        protected string? displayName;

        public BaseObject(IWebDriver driver, Report report, string? displayName = null)
        {
            this.driver = driver;
            this.report = report;
            this.displayName = displayName;

        }

        protected abstract ISearchContext Root();

        //public access to the root element.
        //This is separate to just making Root() public to discourage use, but still's necessary for some model interactions.

        public ISearchContext PublicRoot() => Root();

        public LogHelper Log()
        {
            return new LogHelper(GetSourceNames(), report);
        }

        //<Summary>
        //Return an 'InlineAssert' object that can be used to perform assertions without breaking the fluent chain.
        //</Summary>

        public InlineAssert<TSelf> Assert => new((TSelf)this, report);

        internal virtual string GetClassName()
        {
            var name = GetType().Name;
            //Templates types contain ugly "<number>" characters at the end, which aren't necessary in the report.
            if (name.Contains('`'))
            {
                name = name.Split('`')[0];
            }
            return displayName is null ? name : $"{name} ({displayName})";
        }

        public virtual List<string> GetSourceNames()
        {
            return new() { GetClassName() };
        }

        public TSelf ExpandSection(string text)
        {
            var element = new Wait(driver).Until( d =>
                Root().FindElements(Selectors.Section(text)).First(e => e.Displayed)
            );

            if (element.GetAttribute("aria-expanded").Equals("false"))
            {
                new Wait(driver).UntilClickable(element).Click();
            }

            return (TSelf)this;
        }

        public TSelf CollapseSection(string text)
        {
            var element = new Wait(driver).Until( d =>
                Root().FindElement(Selectors.Section(text))
            );

            if (!element.GetAttribute("aria-expanded").Equals("false"))
            {
                new Wait(driver).UntilClickable(element).Click();
            }

            return (TSelf)this;
        }

        public string GetInputValue(string label, string attribute = "value")
        {
            try
            {
                var value = new Wait(driver,5).Until(d => Root()
                        .FindElements(Selectors.Input(label))
                        .First(e => e.Displayed)
                    )
                    .GetAttribute(attribute);
                Log().Value($"{label} (Editable field)", value); 
                return value;   
            }
            catch (WebDriverTimeoutException)
            {
                var value = new Wait(driver, 5).Until(d => Root()
                        .FindElements(Selectors.UneditableField(label))
                        .First(e => e.Displayed)
                    )
                    .Text;
                Log().Value($"{label} (Uneditable field)", value);
                return value;  
            }
        }

        public double GetInputNumericValue(string label, string attribute = "value")
        {
            var value = GetInputValue(label, attribute);
            var num = string.IsNullOrEmpty(value) ? 0.0 : Convert.ToDouble(value);
            return num;
        }

        public TSelf StoreInputValue(string label, out string output)
        {
            output = GetInputValue(label);
            return (TSelf)this;
        }

        public TSelf Store<T>(Func<TSelf, T> getter, out T ouput, string? logName = null)
        {
            output = getter((TSelf)this);
            if (logName is not null) Log().Value(logName, output);
            return (TSelf)this;
        }

        public TSelf SetInput(string label, string value, string attribute = "value", bool clickElement = true)
        {
            return SetInput(label, label, value, attribute,clickElement);
        }

        public TSelf SetInput(string displayName, string label, string value, string attribute = "value", bool clickElement = true)
        {
            Log().Set(displayName,value); //TODO check if all inputs have a "name" instead

            new Wait(driver).Retry(d =>
                {
                    var element = Root()
                        .FindElements(Selectors.Input(label))
                        .Where(e => e.Displayed && !"true".Equals(e.GetAttribute("readonly")))
                        .Single();
                    var elementText = element.GetAttribute(attribute);

                    if (clickElement && !driver.SwitchTo().ActiveElement().Equals(element)) element.Click();
                    
                    if (elementText != null)
                    {
                        element.SendKeys(new StringBuilder(elementText.Length).Insert(0, Keys.ArrowRight, elementText.Length).ToString());
                        element.SendKeys(new StringBuilder(elementText.Length).Insert(0, Keys.Backspace, elementText.Length).ToString());
                    } 

                    element.SendKeys(value);                  
                        
                }
            );
            return (TSelf)this;
        }

        public TSelf SetInput(string displayName, IWebElement element, string value, string attribute = "value", bool clickElement = true)
        {
            Log().Set(displayName,value); //TODO check if all inputs have a "name" instead

            new Wait(driver).Retry(d =>
                {
                    
                    var elementText = element.GetAttribute(attribute);

                    if (clickElement && !driver.SwitchTo().ActiveElement().Equals(element)) element.Click();
                    
                    if (elementText != null)
                    {
                        element.SendKeys(new StringBuilder(elementText.Length).Insert(0, Keys.ArrowRight, elementText.Length).ToString());
                        element.SendKeys(new StringBuilder(elementText.Length).Insert(0, Keys.Backspace, elementText.Length).ToString());
                    } 

                    element.SendKeys(value);                  
                        
                }
            );
            return (TSelf)this;
        }

        public Flyout<TSelf> ExpandFlyout(string name)
        {
            Log().Click($"Flyout: {name}");
            var element = new Wait(driver,15).Until(d =>
            {
                Root()
                    .FindElements(Selectors.Button(name))
                    .First(e => e.GetAttribute("class").Contains("flyoutButton-Button"))
                    .Click();

                return new Wait(d, 3).Until(d => d
                    .FindElements(By.ClassName("sysPopup"))
                    .First(e => e.Displayed));          
            });
            return new(
                element,
                (TSelf)this,
                report,
                name
            );
        }

        public TSelf SetCombobox(string label, string value)
        {
            return SetInput(label, label, value,"title");
        }

        public TSelf SetCombobox(string displayName, string label, string value)
        {
            return SetInput(displayName, label, value, "title");
        }

        public TSelf SetCombobox(string displayName,IWebElement element, string value)
        {
            return SetInput(displayName, element, value, "title");
        }

        public TSelf ToggleSwitch(string switchName)
        {
            Root().FindElement(Selectors.Switch(switchName)).Click();
            return (TSelf)this;
        }

        public TSelf SetSwitchIfExists(string switchName, bool value)
        {
            try
            {
               SetSwitch(switchName,value);
            }
            catch (NoSuchElementException) { }

            return (TSelf)this;
        }

        public TSelf SetSwitch(string switchName, bool value)
        {
            var switchElement = Root().FindElement(Selectors.Switch(switchName));
            Log().Set($"Switch '{switchName}'", value ? "On" : "Off");

            return SetSwitch(switchElement, value);
        }

        protected TSelf SetSwitch(IWebElement switchElement, bool value)
        {
            if (!switchElement.GetAttribute("aria-checked").Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                switchElement.Click();
                new Wait(driver, 5).ForBlocking();
            }

            return (TSelf)this;
        }

        private bool MatchOperationMessage(IWebElement element, Should should, string text)
        {
            if (!element.Displayed) return false;
            switch (should)
            {
                case Should.Equal:
                    return element.Text.Equals(text, StringComparison.CurrentCultureIgnoreCase);
                case Should.Contain:
                    return element.Text.Contains(text, StringComparison.CurrentCultureIgnoreCase);
                case Should.Match:
                    return Regex.Match(element.Text, text).Success;
                default:
                    throw new Exception("Invalid `Should`");
            }
        }

        public OperationMessage<TSelf>? GetMessage(Should should, string text)
        {
            try
            {
                return new OperationMessage<TSelf>(
                    new Wait(driver, 5).Until(d => d
                        .FindElements(By.ClassName("messageBar-messageEntry"))
                        .First(e => MatchOperationMessage(e, should, text))
                    ),
                    (TSelf)this,
                    report
                );
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public TSelf ExpandMessages()
        {
            Log().Info("Expand messages dropdown");

            new Wait(driver).Retry(d => d
                .FindElements(By.CssSelector("[data-dyn-controlname='MessageBarToggle']"))
                .First(e => e.Displayed)
                .Click()
            );

            return (TSelf)this;
        }

        public MessageDetailsDialog<TSelf> ClickMessageDetails()
        {
            Log().Click("Message details");

            new Wait(driver)
                .Until(d => d
                    .FindElements(By.ClassName("messageBar-detailLink"))
                    .First(e => e.Displayed && e.Text.Equals("Message details")))
                .Click();

            return new(
                new Wait(driver).ForDialogPopup("Message details"),
                (TSelf)this,
                report
            );
        }

        /// <summary>
        /// Open a new tab, open the PDF, and take a screenshot.
        /// Then returns a PdfReader object that can be used to assert on PDF contents, if necessary.
        /// </summary>
        public PdfReader<TSelf> ViewDownloadedPdf(string? name = null)
        {
            PdfReader<TSelf>? pdf = null;
            new Tab(driver, report)
                .Act(() =>
                {
                    var pdfPage = new DownloadsPage(driver, report)
                        .LatestDownload()
                        .OpenPdf();

                    pdfPage.LogScreenshot($"PDF: {(name is not null ? name : "")} {pdfPage.FileName}");

                    pdf = new PdfReader<TSelf>((TSelf)this, pdfPage.FilePath, driver, report);
                })
                .Close();
            return pdf!;
        }

        public TSelf LogDownloadedFiles()
        {
            new Tab(driver, report)
                .Act(() =>
                {
                    var downloads = new DownloadsPage(driver, report)
                        .GetDownloads()
                        .Select(d => d.GetFileName())
                        .ToList();
                    Log().Info("Downloaded the following files: " + Markup.UnorderedList(downloads));
                })
                .Close();
            return (TSelf)this;
        }

        /// <summary>
        /// Wrap a set of test steps in a new test section with a given name.
        /// </summary>
        public R TestSection<R>(string sectionName, Func<TSelf, R> sectionBody)
            where R : BaseObject<R>
        {
            return TestSection(sectionName, "", sectionBody);
        }

        /// <summary>
        /// Wrap a set of test steps in a new test section with a given name and description.
        /// </summary>
        public R TestSection<R>(string sectionName, string description, Func<TSelf, R> sectionBody)
            where R : BaseObject<R>
        {
            report.BeginSection(sectionName, description);
            var r = sectionBody((TSelf)this);
            report.EndSection();
            return r;
        }

        protected void SuppressLogs(Action actions)
        {
            try
            {
                report.TurnOffLogging();
                actions();
            }
            finally
            {
                report.TurnOnLogging();
            }
        }

        protected IWebElement GetSectionElement(string sectionName)
        {
            return driver.FindElements(By.CssSelector("[data-dyn-role='SectionPage']"))
                .First(e => e.Displayed && e.FindElements(Selectors.Button(sectionName)).Any());
        }

        public TSection GetSection<TSection>()
            where TSection : BaseSection<TSelf, TSection>
        {
            return PageActions.ConstructSection<TSection, TSelf>((TSelf)this, report);
        }

        public GenericSection<TSelf> GetSection(string sectionName)
        {
            return new(
                sectionName,
                (TSelf)this,
                report
            );
        }

        /// <summary>
        /// Click link text within an element.
        ///
        /// This is difficult - if the link text is long enough that the centre of the element includes the text,
        /// the initial click will work, resulting in a page or dialog change.
        ///
        /// However, if it's too short, the text will not be clicked, and the input will be "selected". If this happens,
        /// we need to send a Ctrl+Enter key combination (which will fail if the initial click worked, hence the try-catch).
        /// </summary>
        protected void ClickInputLink(IWebElement element)
        {
            element.Click();
            try {
                element.SendKeys(Keys.Control + Keys.Enter);
            } catch {}
        }
    }    
}