using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils
{
    public static class Selectors
    {
       
        // Page including related header and contents        
        static public By Page(string formName) => By.CssSelector($"form[data-dyn-form-name='{formName}']");
 
        // Sidemenu navigation Links        
        static public By ModuleLink(string text) => By.XPath($".//a[contains(@class, 'modulesFlyout-linkText')][text()='{text}']");
        static public By Table(string ariaLabel) => By.CssSelector($"[aria-label*='{ariaLabel}'][role='grid']");
        static public By Button(string text) => By.XPath($".//button[translate(.,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{text.ToLower()}'] | .//button//span[translate(.,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{text.ToLower()}']/../..");

        // Section of groupings in a form        
        static public By Section(string label) => By.CssSelector($"button[aria-label='{label}']");
 
        // Grouping of inputs in a form        
        static public By FormGroup(string group) => By.XPath($".//div[@data-dyn-role='Group' and contains(@data-dyn-controlname, 'group')]//span[contains(text(), '{group}')]/../..");

        // Grouping of buttons that exist in a TopMenu tab and section        
        static public By TopMenuGroup(string text) => By.XPath($".//label[text()='{text}']/../..");

        // Plain text or dropdown input        
        static public By Input(string text) => By.XPath($".//label[text()='{text}']/..//input[@type='text']");
        static public By UneditableField(string text) => By.XPath($".//label[text()='{text}']/..//div[contains(@class, 'field')]");
        static public By InputLink(string text) => By.XPath($".//label[text()='{text}']/..//div[@role='link'] | .//label[text()='{text}']/..//input[@role='textbox']");

        // Checkboxes that exist in dialogs, not DataTable cells        
        static public By Checkbox(string label) => By.XPath($".//label[text()='{label}']/../span[@role='checkbox']");
        static public By Textarea(string text) => By.XPath($".//label[text()='{text}']/..//textarea[@role='textbox']");
        static public By Switch(string text) => By.XPath($".//label[text()='{text}']/..//span[@role='switch']");

        // Last resort selectors        
        static public By Title(string title) => By.XPath($".//*[@title='{title}' or @data-dyn-title='{title}']");
        static public By Name(string name) => By.CssSelector($"[name='{name}' i]");
        static public By HeaderTab(string text) => By.XPath($".//span[text()='{text}']/..");

        // Non-dynamic selectors        
        static public By Lightbox() => By.CssSelector(".lightbox");
        static public By OperationProcessing() => By.CssSelector(".lightbox .activity");
        static public By Dialog() => By.CssSelector(".dialog-popup-content");
        static public By Dialog(string heading) => By.XPath($".//div[contains(@class, 'dialog-popup-content')]//div[@role='heading' and text()='{heading}']/../..");
        static public By Blocking() => By.Id("ShellBlockingDiv");

        // TODO: investigate if this captures all headers on all pages.        
        static public By HeaderTitle() => By.CssSelector("[data-dyn-controlname*='Header'].titleField");
        static public By BusinessUnit() => By.CssSelector("input[name*='BusinessUnit'][role='combobox']");
        static public By CostCentre() => By.CssSelector("input[name*='CostCentre'][role='combobox']");
        /// <summary>        
        /// Helper function to combine multiple FindElements requests into one collection of elements.       
        ///         
        /// Usage:       
        /// ```        
        /// Selectors.Multiple(        
        ///    driver.FindElements(SelectorA),       
        ///    driver.FindElements(SelectorB)        
        /// )       
        /// ```        
        /// </summary>        
        static public IEnumerable<IWebElement> Multiple(params IReadOnlyCollection<IWebElement>[] elementLists)
            => elementLists.SelectMany(x => x);
        /// <summary>       
       /// Combine multiple selectors, expecting one resulting element.      
        /// </summary>        
        static public IWebElement SingleOf(params IReadOnlyCollection<IWebElement>[] elementLists) => Multiple(elementLists).Single();
        }
    }
