using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WebApplication1.Models.Tests
{
    [TestClass()]
    public class OtpObjectTests
    {
        [TestMethod()]
        public void OtpConstructorTest()
        {
            ///Setup
            OtpObject newOtpObject = new OtpObject("123456", "Bob", DateTime.Now.AddSeconds(30), 30, "SUPER_SECRET_KEY",5,0);

            ///Assert
            Assert.IsNotNull(newOtpObject);
            Assert.AreEqual("123456", newOtpObject.Code);
            Assert.AreEqual("Bob", newOtpObject.Username);
            Assert.AreNotEqual(DateTime.Now.AddSeconds(30), newOtpObject.ExpirationDateTime);
            Assert.AreEqual(30, newOtpObject.LifeTime);
            Assert.AreEqual("SUPER_SECRET_KEY", newOtpObject.SecretKey);
            Assert.AreEqual(0, newOtpObject.AttemptsMade);
            Assert.AreEqual(5, newOtpObject.MaxAttempts);

            


        }

        [TestMethod()]
        public void OtpSetTest()
        {
            ///Setup
            OtpObject newOtpObject = new OtpObject("123456", "Bob", DateTime.Now.AddSeconds(30), 30, "SUPER_SECRET_KEY",5,0);
            Assert.IsNotNull(newOtpObject);

            ///Act
            newOtpObject.Code = "789101";
            newOtpObject.Username = "Steff";
            newOtpObject.ExpirationDateTime= DateTime.Now.AddSeconds(50);
            newOtpObject.LifeTime = 50;
            newOtpObject.SecretKey = "NEW_SUPER_SECRET_KEY";
            newOtpObject.MaxAttempts = 10;
            newOtpObject.AttemptsMade = 3;


            ///Assert
            Assert.AreEqual("789101", newOtpObject.Code);
            Assert.AreEqual("Steff", newOtpObject.Username);
            Assert.AreNotEqual(DateTime.Now.AddSeconds(50), newOtpObject.ExpirationDateTime);
            Assert.AreEqual(50, newOtpObject.LifeTime);
            Assert.AreEqual("NEW_SUPER_SECRET_KEY", newOtpObject.SecretKey);
            Assert.AreEqual(3, newOtpObject.AttemptsMade);
            Assert.AreEqual(10, newOtpObject.MaxAttempts);


        }
    }
}