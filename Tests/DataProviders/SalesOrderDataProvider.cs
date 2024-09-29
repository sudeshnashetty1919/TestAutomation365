using System.Collections;
using System.Collections.Generic;
using dynamics365accelerator.Model.Data;
using NUnit.Framework;

namespace dynamics365accelerator.Tests.DataProviders
{
    public class SalesOrderDataProvider : IEnumerable<TestCaseData>
    {
        public IEnumerable<TestCaseData> GetEnumerator()
        {
            //TODO read this from DB
            var item1 = new OrderItem("24310",2,"29C");
            var item2 = new OrderItem("0101",2,"27C");
            var item3 = new OrderItem("10036",2,"13C");
            var item4 = new OrderItem("5050BNZ",2,"10D");
            var order = new SalesOrder(CustomerAccount: "4004085", CustomerReference:"RSB5.01",Items: new List<OrderItem>{item1, item2, item3, item4});
            //TODO Write to DB or API
            return new List<TestCaseData> {new TestCaseData(order).SetName("{c} - {m}")}.GetEnumerator();        
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }


}