using dynamics365accelerator.Model.Components.Dialogs;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Sections
{
    public class AddressSection<TParent> : BaseSection<TParent, AddressSection<TParent>>
        where TParent : BaseObject<TParent>
    {
        public AddressSection(TParent parent, Report report)
            : base("Address", parent, report)
        {
        }

        public AddressSelectionDialog<AddressSection<TParent>> ClickOtherAddress()
        {
            ClickButton("Other address");

            return new(
                new Wait(driver).ForDialogPopup("Address selection"),
                this,
                report
            );
        }//Completed
    }
}