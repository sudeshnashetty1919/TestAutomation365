using System.Collections;
using System.Collections.Generic;
using dynamics365accelerator.Model.Data;

//dynamics365accelerator == dynamics365accelerator

namespace dynamics365accelerator.Tests.DataProviders
{
    public class InventoryAdjustmentDataProvider : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            //yield return new TestCaseData(Data).SetName("{i} -  {m}");
            yield return Data;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static object[] Data = {
            new object[] {
                new InventoryAdjustmentData("Automation RSB11.01", "29C", new List<InventoryJournalLineData>{
                    new InventoryJournalLineData("CRC5089",-1,"SRDEND","NZTS","13129","29C","SRDEND"),
                    new InventoryJournalLineData("2053",-1,"SRW3","NZTS","13129","29C","SRW3")
                })
            }
        };
    }    
    
}