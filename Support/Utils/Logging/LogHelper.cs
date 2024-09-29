using System.Collections.Generic;
using System.Linq;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using OpenQA.Selenium;

namespace dynamics365accelerator.Support.Utils.Logging
{
    //<Summary>
    //Wrapper to support unified logging within all tests
    //</Summary>
    public class LogHelper
    {
        private readonly Report report;
        private readonly List<string> sources;

        public LogHelper(List<string> sources, Report report)
        {
            this.report = report;
            this.sources = sources;
        }

        //<Summary>
        //Get a color to assign to a source label, given its' depth
        //</Summary>
        ///<param name = "sourceDepth">
        //The depth of the source label. The first source label is 0, the next is 1, and so on.
        //<param>
        //<return> The colour to assign to the label.</returns>

        private ExtentColor GetSourceColour(Status status, string sourceName, int sourceDepth)
        {
            if (status.Equals(Status.Pass) && (sourceName.Contains("Assert") || sourceName.Contains("Warning")))
            {
                //Asserts are green
                return ExtentColor.Green;
            }

            if (status.Equals(Status.Warning) &&  sourceName.Contains("Warning"))
            {
                //Warnings are Orange
                return ExtentColor.Red;
            }

            if (sourceDepth == 0)
            {
                //Classes are lime
                return ExtentColor.Lime;
            }

            //Components inside classes are amber
            return ExtentColor.Amber;
        }
        private void InternalLog(Status status, string Text)
        {
            //Generate the multiple source labels.
            var sourceLabels = string.Join(
                " > ",
                sources.Select((sourceName, sourceDepth) =>
                    MarkupHelper.CreateLabel(sourceName, GetSourceColour(status, sourceName, sourceDepth)).GetMarkup()

                )
            );
            report.Test.Log(status, $"{sourceLabels} {text}");
        }

        public void DataSelected<T>(string name, T value)
        {
            report.LogTestData($"Selected: <b>{name}</b>", value);
        }

        public void DataCreated<T>(string name, T value)
        {
            report.LogTestData($"Created: <b>{name}</b>", value);
        }

        public void DataSource<T>(string name, T value)
        {
            report.LogTestData($"<b>{name}</b>", value);
        }

        public void Log(Status status, string text)
        {
            InternalLog(status, text);
        }

        public void Info(string text)
        {
            InternalLog(Status.Info, text);
        }

        public void Pass(string text)
        {
            InternalLog(Status.Pass, text);
        }

        public void Warn(string text)
        {
            InternalLog(Status.Warning, text);
        }

        public void Click(string name)
        {
            Log(Status.Info, $"Click '<strong>{name}</strong>'");
        }

        //<summary>
        //Log a "Set" operation
        //</summary>

        public void Set<T>(string name, T to)
        {
            Log(Status.Info, $"Set '<strong>{name}</strong>' to '<strong>{FormatValue(to)}</strong>'");
        }

        //<summary>
        //Log a value
        //</summary>
        public T Value<T>(string name, T value)
        {
            Log(Status.Info, $"'<strong>{name}</strong>' is '<strong>{value}</strong>'");
            return value;
        }

        //<summary>
        //Log a value
        //</summary>
        public void Value<T>(string name, List<T> list)
        {
            var formattedList = string.Join(",", list.Select(v => $"'<b>'{v}'</b>'"));
            Log(Status.Info, $"List'<strong>{name}</strong>' is [{formattedList}]");
            
        }

        private string FormatValue<T>(T value)
        {
            return value
                .ToString()
                .Replace(Keys.Tab,"")
                .Replace(Keys.Tab,"");
        }
    }  //Completed expect 2 symbols in 139 and 140
}
