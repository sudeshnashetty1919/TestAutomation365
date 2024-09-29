using System;
using System.Linq;
using dynamics365accelerator.Model.Components.Features;
using dynamics365accelerator.Support.Utils;
using dynamics365accelerator.Support.Utils.Logging;
using OpenQA.Selenium;

namespace dynamics365accelerator.Model.Components.Sections
{
    public class FinancialDimensionsSection<TParent> : BaseSection<TParent, FinancialDimensionsSection<TParent>>
        where TParent : BaseObject<TParent> 
    {
        public FinancialDimensionsSection(TParent parent, Report report)
            : base("Financial dimensions", parent, report)
        {
        }

        protected FinancialDimensions<FinancialDimensionsSection<TParent>> FinancialDimensions => new(Root(), this, report);
        public string GetBusinessUnit() => FinancialDimensions.GetBusinessUnit();
        public string GetCostCentre() => FinancialDimensions.GetCostCentre();
        public FinancialDimensionsSection<TParent> SetBusinessUnit(string businessUnit) => FinancialDimensions.SetBusinessUnit(businessUnit);
        public FinancialDimensionsSection<TParent> SetCostCentre(string costCentre) => FinancialDimensions.SetCostCentre(costCentre);
    }
    //Completed
}