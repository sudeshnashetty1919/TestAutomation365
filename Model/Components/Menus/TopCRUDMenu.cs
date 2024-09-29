using OpenQA.Selenium;
using System.Threading;
using System.Linq;
using dynamics365accelerator.Support.Utils.Logging;
using dynamics365accelerator.Support.Utils;

namespace dynamics365accelerator.Model.Components.Menus
{
    /// <summary>
    /// A helper class to contain common interactions with the top menu.
    /// This is an internal class, and should not be exposed in the test.
    /// Instead, if common top menu interactions are needed, they can be added to `BaseCRUDPage`
    /// and use the internal `TopMenu()` function to interact with this class.
    /// </summary>
    /// <typeparam name="TParent">The parent page that the TopCRUDMenu is contained in</typeparam>
    public class TopCRUDMenu<TParent> : BaseComponent<TParent, TopCRUDMenu<TParent>>
        where TParent : BaseObject<TParent>
    {
        public TopCRUDMenu(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement, parent, report) { }

        public void ClickCRUDButton(string text)
        {
            Log().Click(text);
            new Wait(driver, 10).Retry(d =>
                Root().FindElement(Selectors.Button(text)).Click()
            );
            new Wait(driver).ForBlocking();
        } 

        public void ClickCRUDButtonInGroup(string group, string text)
        {
            Log().Click($"{group} > {text}");

            new Wait(driver, 10).Retry(d =>
                Root()
                    .FindElements(Selectors.TopMenuGroup(group))
                    .Where(group => group.Displayed)
                    .First()
                    .FindElement(Selectors.Button(text))
                    .Click()
            );
        }

        public void ClickNew()
        {
            ClickCRUDButton("New");
        }

        public TParent ClickSave()
        {
            ClickCRUDButton("Save");
            return parent;
        }

        public TParent ClickEdit()
        {
            ClickCRUDButton("Edit");
            return parent;
        }

        public TParent ClickRefresh()
        {
            ClickCRUDButton("Refresh");
            return parent;
        }

        // TODO: The close button doesn't always lead to the parent
        public TParent ClickClose()
        {
            ClickCRUDButton("Close");
            // This wait is here to ensure the page is actually closed before a test continues
            // Unfortunately, it's not consistent, due to the way that certain pages - mainly the "list" and "edit"
            // devices that are associated with each other, like `AllDevicesPage` and `EditDevicePage` -  have the same shared root.
            // For this reason, we don't fail on timeout.
            try
            {
                new Wait(driver, 5).Until(page =>
                {
                    try
                    {
                        return !Root().Displayed;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
            }
            catch (WebDriverTimeoutException) { }

            return parent;
        }  

        public TParent ClickTab(string controlName)
        {
            new Wait(driver, 5).Retry(d =>
                Root()
                    .FindElements(By.CssSelector($"[data-dyn-role='AppBarTab']"))
                    .Where(e => e.Text.Equals(controlName))
                    .First()
                    .Click()
            );
            return parent;
        }

        //Completed 
    }
}