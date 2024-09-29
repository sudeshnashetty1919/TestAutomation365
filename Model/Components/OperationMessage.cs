using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components
{
    //Models the message that appear in the message bar
    public class OperationMessage<TParent> : BaseComponent<TParent, OperationMessage<TParent>>
        where TParent : BaseObject<TParent>
    {
        public OperationMessage(IWebElement rootElement, TParent parent, Report report)
            : base(rootElement,parent,report)
        {
            Log().Info($"Found message: '<b>{Text}</b>'"); 

        }

        public string Text => Root().Text;    
    //Completed
    }
}