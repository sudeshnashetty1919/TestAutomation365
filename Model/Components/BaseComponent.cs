using System;
using System.Collections.Generic;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components
{
    public abstract class BaseComponent<TParent, TSelf> : BaseObject<TSelf>
       where TParent : BaseObject<TParent>
       where TSelf : BaseObject<TSelf>
    {
        private readonly IWebElement rootElement;
        protected readonly TParent parent;

        public TParent Parent => parent;
        
        public BaseComponent(IWebElement rootElement, TParent parent, Report report, string? displayName = null)
            : base(((IWrapsDriver)rootElement).WrappedDriver, report, displayName)
        {
            this.rootElement = rootElement;
            this.parent = parent;
        }    

        protected override IWebElement Root() => rootElement;

        public void ClickButton(string buttonText, string additionalInfo = "")
        {
            Log().Click($"{buttonText} [Button] {additionalInfo}");

            Root().FindElement(Selectors.Button(buttonText)).Click();

            new Wait(drier, 120).ForBlocking();
        }

        public override List<string> GetSourceNames()
        {
            var hierarchy = parent.GetSourceNames();
            hierarchy.Add(GetClassName());
            Console.WriteLine(hierarchy);
            return hierarchy;
        }

        //Completed

    } 
}