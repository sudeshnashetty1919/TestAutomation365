using dynamics365accelerator.Model.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
//dynamics365accelerator = dynamics365accelerator
namespace dynamics365accelerator.Tests.DataProviders
{
    public class UserContextDataProvider
    {
        public static List<UserContextData> ReadUserData()
        {
            var filePath = @"Tests\DataProviders\UserContextData.json";

            using (StreamReader sr = new StreamReader(filePath))
            {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<List<UserContextData>>(json)!;

            }
        }

    }

}