using dynamics365accelerator.Model;
using dynamics365accelerator.Model.Components;
using dynamics365accelerator.Model.Components.Sections;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils
{
    public static class PageActions
    {

        ///<Summary>
        //Construct an arbitrary page T.
        ///</summary>

        static public T ConstructPage<T>(IWebDriver driver, Report report)
            where T: BaseObject<T>
        {
            return (T)typeof(T)
                .GetConstructor(new[] {typeof(IWebDriver), typeof(Report)})
                .Invoke(new object[] {driver, report});
        }
        
        ///<Summary>
        //Construct an component TComponent.
        ///</summary>
        static public TComponent ConstructComponent<TComponent,TParent>(IWebElement rootElement, TParent parent, Report report)
            where TComponent : BaseComponent<TParent, TComponent>
            where TParent : BaseObject<TParent>
        {
            return (TComponent)typeof(TComponent)
                .GetConstructor(new[] { typeof(IWebElement),typeof(TParent), typeof(Report) })
                .Invoke(new object[] { rootElement,parent, report });

        }

        ///<Summary>
        //Construct an component TComponent where TComponent has a Tnext parameter.
        ///</summary>
        static public TComponent ConstructComponent<TComponent,TNext,TParent>(IWebElement rootElement, TParent parent, TNext nextPage,Report report)
            where TComponent : BaseComponent<TParent, TComponent>
            where TParent : BaseObject<TParent>
            where TNext : BaseObject<TNext>
        {
            return (TComponent)typeof(TComponent)
                .GetConstructor(new[] { typeof(IWebElement),typeof(TParent), typeof(TNext),typeof(Report) })
                .Invoke(new object[] { rootElement,parent, report });

        }

        ///<Summary>
        //Construct an arbitrary section
        //Note that Section titles are determined by the section class itself.
        ///</summary>
        static public T ConstructSection<T,TParent>(TParent parent, Report report)
            where T : BaseSection<TParent, T>
            where TParent : BaseObject<TParent>
            
        {
            return (T)typeof(T)
                .GetConstructor(new[] { typeof(TParent), typeof(Report) })
                .Invoke(new object[] { parent, report });

        }
        // <PackagaeReference Include="ErraticMotion.Spock.Net" Version="0.1.0" />
        //<PackagaeReference Include="iTextSharp" Version="5.5.13.3" />
    }
}