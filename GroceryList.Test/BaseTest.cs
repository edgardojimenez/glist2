using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
namespace GroceryList.Test  {
    [TestClass]
    public abstract class TestBase {
 
        protected virtual void TestSetup() { }
        protected virtual void TestTearDown() { }

        protected virtual void ClassSetup() { }
        protected virtual void ClassTearDown() { }
 
        public TestContext TestContext { get; set; }
 
        [TestInitialize]
        public void TestInitialize() {
            TestSetup();
        }

        [TestCleanup]
        public void TestCleanup() {
            TestTearDown();
        }

        
 
        public decimal RandomNumber() {
            var random = new Random();
            return (decimal)random.Next(1, 100);
        }
    }
}
