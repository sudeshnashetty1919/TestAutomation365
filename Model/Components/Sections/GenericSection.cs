using dynamics365accelerator.Support.Utils.Logging;

namespace dynamics365accelerator.Model.Components.Sections
{
    public class GenericSection<TParent> : BaseSection<TParent, GenericSection<TParent>>
        where TParent : BaseObject<TParent>
    {
        public GenericSection(string sectionName, TParent parent, Report report)
            : base(sectionName, parent, report, sectionName)
        {
        }
        //Completed
    }
}