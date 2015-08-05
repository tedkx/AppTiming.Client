using System;
using NUnit.Framework;

namespace AppTiming.Client.Tests
{
    [TestFixture]
    public class HelperTests
    {
        public void Helper_Start_MustReturnCode()
        {

        }

        public void Helper_End_MustReturnValidObject()
        {

        }

        [Test]
        [TestCase("http://192.168.0.21:3000/api/v1/")]
        public void Helper_ValidEndpoint_MustNotThrowException(object arg1)
        {
            var endpoint = arg1 as string;
            Exception exception = null;

            try
            {
                var client = new AppTimingClient("dummy", endpoint);
            }
            catch (Exception ex) { exception = ex; }

            Assert.IsNull(exception, "Exception thrown for valid endpoint: " + (exception != null ? exception.Message : string.Empty));
        }

        [Test]
        [TestCase("", typeof(ArgumentNullException))]
        [TestCase("htt:/asdf", typeof(ArgumentException))]
        public void Helper_InvalidEndpoint_MustThrowException(object arg1, object arg2)
        {
            string invalidEndpoint = arg1 as string;
            Type exceptionType = arg2 as Type;
            object exception = null;

            try
            {
                var client = new AppTimingClient("dummy", invalidEndpoint);
            }
            catch(Exception ex) { exception = ex; }

            Assert.IsNotNull(exception, "No exception thrown for invalid endpoint " + invalidEndpoint);
            Assert.AreEqual(exception.GetType(), exceptionType, "Wrong exception thrown for invalid endpoint " + invalidEndpoint);
        }

        
    }
}
