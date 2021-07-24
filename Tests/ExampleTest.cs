using System;
using System.Diagnostics;
using eCommerce;
using NUnit.Framework;

namespace Tests
{
    public class ExampleTests
    {
        [SetUp]
        public void Setup()
        {
            //Item item = new Item();
            Debug.WriteLine("Dor");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        
        [TearDown]
        public void TearDown()
        {
            
        }
    }
}