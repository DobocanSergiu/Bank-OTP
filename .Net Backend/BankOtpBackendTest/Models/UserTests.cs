using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models.Tests
{
    [TestClass()]
    public class UserTests
    {
        [TestMethod()]
        public void UserConstructorTest()
        {
            ///Setup
            User testUser = new User(123,"Mike","TEST_HASH","TEST_SALT");  
            ///Assert
            Assert.IsNotNull(testUser);
            Assert.AreEqual(123, testUser.Id);
            Assert.AreEqual("Mike", testUser.Username);
            Assert.AreEqual("TEST_HASH", testUser.PasswordHash);
            Assert.AreEqual("TEST_SALT", testUser.SaltBase64String);

        }

        [TestMethod()]
        public void UserSetTest()
        {
            ///Setup
            User testUser = new User(123, "Mike", "TEST_HASH", "TEST_SALT");
            ///Act
            testUser.Id = 22;
            testUser.Username = "Bob";
            testUser.PasswordHash = "NEW_TEST_HASH";
            testUser.SaltBase64String = "NEW_SALT_STRING";
            ///Assert
            Assert.IsNotNull(testUser);
            Assert.AreEqual(22, testUser.Id);
            Assert.AreEqual("Bob", testUser.Username);
            Assert.AreEqual("NEW_TEST_HASH", testUser.PasswordHash);
            Assert.AreEqual("NEW_SALT_STRING", testUser.SaltBase64String);


        }
    }
}