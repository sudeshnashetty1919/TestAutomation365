using System;
using System.Collections.Generic;
using System.Linq;
using AventStack.ExtentReports;
using NUnit.Framework;
 
namespace dynamics365accelerator.Support.Utils.Logging
{
    public class Report
    {
        public ExtentReports extent;
        //The top-level report object.
        private readonly ExtentTest rootReport;
        //The root-level test steps node.
        private readonly ExtentTest rootTest;
        //Test sections are modelled as a stack. New nodes/section are added to the top, and when ending a section it is removed.
        private readonly Stack<ExtentTest> sectionStack;
        //sends all test logs to the current section (the top of the section stack).
        //if shouldLog is false, instead it constructs a useless test report that is never wtitten to a file.

        public ExtentTest Test => disableLogs ? new ExtentReports().CreateTest("Null") : sectionStack.Peek();

        public ExtentTest Data { get; }

        //To prevent double-logging to the data section, we keep track of every string logged and prevent logging of strings that have already been seen.

        private readonly HashSet<string> loggedData;

        private bool disableLogs = false;
        public Report (ExtentReports extent)
        {
            this.extent = extent;

            var descriptionAttribute = (string?)TestContext.CurrentContext.Test.Properties.Get("Description");
            var description = descriptionAttribute is null ? "" : descriptionAttribute;

            rootReport = extent.CreateTest(TestContext.CurrentContext.Test.FullName, description);

            var categories = TestContext.CurrentContext.Test.Properties["Category"];
            foreach (object category in categories)
            {
                rootReport.AssignCategory((string)category);

            }

            Data = rootReport.CreateNode("Data");
            rootTest = rootReport.CreateNode("Test");
            sectionStack = new Stack<ExtentTest>();
            sectionStack.Push(rootTest);
            loggedData = new HashSet<string>();
        }

        public string FormatValue<T>(T value)
        {
            try{
                return ((IlogFormat)value!).FormatForLog();
            }
            catch (InvalidCastException)
            {
                return value.ToString();

            }
        }

        private string FormatList<T>(List<T> list)
        {
            var strings = list.Select(x => FormatValue(x)).ToList();
            //Determine whether to format as multi - line or single - line list.
            var formatMultiLine = strings.Where(item => item.Length > 40).Any() || list.Count > 3;
            return formatMultiLine ? Markup.UnorderedList(strings!) : string.Join(",",strings);
        }

        public List<T> LogTestData<T>(string name, List<T> testData)
        {
            LogTestData(name, FormatList(testData));

            return testData;
        }

        public T LogTestData<T>(string name, T testData)
        {
            string s = "";
            if (testData is null)
            {
                s = $"{name}: not present";
            }
            else if (testData.GetType().GetInterfaces().Any( x =>
                x.IsGenericType
                && x.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                var list = ((IReadOnlyList<object>)testData).Select(x => FormatValue(x)).ToList();
                s = $"{name}: {FormatList(list)}";
            } 
            else
            {
                s = $"{name}: {FormatList(testData)}";

            }  

            if (!string.IsNullOrEmpty(s) && !loggedData.Contains(s))
            {
                Data.Info(s);
                loggedData.Add(s);
            } 

            return testData;
        }

        /// <Summary>
        /// Begin a section with the given name and description.
        /// </Summary>
        public void BeginSection(string name, string description = "")
        {
            sectionStack.Push(Test.CreateNode(name, description));
        }

        /// <Summary>
        /// Edn the current test section. All subsequent logs will go to the parent section.
        /// </Summary>

        public void EndSection()
        {
            sectionStack.Pop();
        }

        public void TurnOffLogging()
        {
            disableLogs = true;
        }

        public void TurnOnLogging()
        {
            disableLogs = false;
        }

        public void Skip(string message)
        {
            // On skipping a test, we remove all test content
            Data.Skip("");
            Test.Skip(message);
        }

        //Completed
 
    }
}