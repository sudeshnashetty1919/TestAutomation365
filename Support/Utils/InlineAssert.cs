using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using dynamics365accelerator.Model;
using dynamics365accelerator.Model.Data.Enums;
using dynamics365accelerator.Support.Utils.Logging;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace dynamics365accelerator.Support.Utils
{
    public class InlineAssert<T>
        where T : BaseObject<T>
    {
        private readonly T underlying;

        private readonly Report report;

        public InlineAssert(T underlying, Report report)
        {
            this.underlying = underlying;
            this.report = report;
        }

        public InlineAssert<T> After(TimeSpan timespan)
        {
            Thread.Sleep(Convert.ToInt32(timespan.TotalMilliseconds));
            return this;
        }

        protected LogHelper LogAssert()
        {
            return new LogHelper(underlying.GetSourceNames().Concat(new List<string> {"Assert"}).ToList(), report);
        }

        protected LogHelper LogWarn()
        {
            return new LogHelper(underlying.GetSourceNames().Concat(new List<string> {"Warning"}).ToList(), report);
        }

        public T ThatOperationMessage(Should should, string expected)
        {
            var assertMessage = $"Expected operation message {should.AsString()}: '<b>{expected}</b>'";
            if (should == Should.NotExist)
            {
                var message = underlying.GetMessage(Should.Equal, expected);
                if (message is not null)
                {
                    Assert.Fail($"{assertMessage}, but found '<b>{message.Text}</b>'");
                }
            }
            else
            {
                var message = underlying.GetMessage(should, expected);
                if (message is null)
                {
                    Assert.Fail($"{assertMessage}, but no message was found.");
                }
            }
            LogAssert().Pass(assertMessage);
            return underlying;
        }

        public T AreNotEqual<R1, R2>(Func<T, R1> expected, Func<T, R2> actual, string? message = null)
        {
            Assert.AreNotEqual(expected, actual, message);

            LogAssert().Pass($"Expected: <b>{actual(underlying)}</b> not to be equal to <b>{expected(uderlying)}</b>");

            return uderlying;
        }

        public T AreNotEqual<R1, R2>(R1 expected, Func<T, R2> actual, string? message = null)
        {
            Assert.AreNotEqual(expected, actual(underlying), message);

            LogAssert().Pass($"Expected: <b>{actual(underlying)}</b> not to be equal to <b>{expected}</b>");
            
            return uderlying;
        }

        public T AreNotEqual<R1, R2>(R1 expected, R2 actual, string? message = null)
        {
            Assert.AreNotEqual(expected, actual, message);

            LogAssert().Pass($"Expected: <b>{actual}</b> not to be equal to <b>{expected}</b>");
            
            return uderlying;
        }

        public T AreEqual<R1, R2>(Func<T, R1> expected, Func<T, R2> actual, string message)
        {
            Assert.AreEqual(expected(uderlying), actual(underlying), message);

            LogAssert().Pass($"Expected: <b>{actual(underlying)}</b> to be equal to <b>{expected(uderlying)}</b>");

            return uderlying;
        }

        public T AreEqual<R1, R2>(R1 expected, Func<T, R2> actual, string message)
        {
            Assert.AreEqual(expected, actual(underlying), message);

            LogAssert().Pass($"Expected: <b>{actual(underlying)}</b> to be equal to <b>{expected}</b>");
            
            return uderlying;
        }

        public T AreEqual<R1, R2>(R1 expected, R2 actual, string message)
        {
            Assert.AreEqual(expected, actual, message);

            LogAssert().Pass($"Expected: <b>{actual}</b> to be equal to <b>{expected}</b>");
            
            return uderlying;
        }

        public T That<R>(Func<T, R> subject, IResolveConstraint constraints, string message)
        {
            var result = subject(underlying);
            Assert.That(result, constraint,message);

            LogAssert().Pass($"Expected: <b>{constraints.Resolve().Description}</b>");
            return underlying;
        }

        public T That<R>(R subject, IResolveConstraint constraints, string message)
        {
           
            Assert.That(subject, constraint,message);

            LogAssert().Pass($"Expected: <b>{constraints.Resolve().Description}</b>");

            return underlying;
        }

        public T That(Func<T, bool> subject, string? message = null)
        {
            var result = subject(underlying);
            
            Assert.That(result,message);

            LogAssert().Pass($"Passed");

            return underlying;
        }

        public T WarnUnless<R>(Func<T, R> subject, IResolveConstraint constraint, string message)
        {
            var result = constraint.Resolve().ApplyTo(subject(underlying));
            if (result.IsSuccess)
            {
                LogWarn().Pass($"Expected: <b>{result.Description}</b>, Found: '<b>{result.ActualValue}</b>'");
            }
            else
            {
                LogWarn().Pass($"Expected: <b>{result.Description}</b>, Found: '<b>{result.ActualValue}</b>' - {message}");
            }
        }

        public T EnsureExpected<R>(Func<T, R> subject, IResolveConstraint constraint, Func<T , object> ifNotExpected, string message)
        {
            var result = constraint.Resolve().ApplyTo(subject(underlying));
            if (result.IsSuccess)
            {
                LogWarn().Pass($"Expected: <b>{result.Description}</b>, Found: '<b>{result.ActualValue}</b>'");
            }
            else
            {
                LogWarn().Pass($"Expected: <b>{result.Description}</b>, Found: '<b>{result.ActualValue}</b>'{(message is null ? "" : $" - {message}")}. Setting field to expected value...");
                ifNotExpected(underlying);
            }
            return underlying;
        }

        public T Multiple(Action<InlineAssert<T>> assertions)
        {
            Assert.Multiple( () => assertions(this));
            return underlying;
        }

        public T Multiple(Action<T, InlineAssert<T>> assertions)
        {
            Assert.Multiple( () => assertions(underlying,this));
            return underlying;
        }

        public T IsNotNull<R>(Func<T, R> subject, string message)
        {
            var result = subject(underlying);

            Assert.That(result, Is.Not.Null, message);

            LogAssert().Pass($"Expected: {result} not to be null");

            return underlying;

        }

        //Completed
            
        
    }     
        
        
}
