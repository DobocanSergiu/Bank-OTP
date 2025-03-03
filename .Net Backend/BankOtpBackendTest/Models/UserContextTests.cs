using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Tests
{
    [TestClass()]
    public class UserContextTests
    {
        private DbContextOptions<UserContext> GetInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [TestMethod()]
        public void AddUserTest()
        {

            ///Setup
            var options = GetInMemoryDatabaseOptions();
            var context = new UserContext(options);

            ///Act
            context.Users.Add(new Models.User(1,"Bob","PASSWORD_HASH_32","SALT_32"));
            context.Users.Add(new Models.User(2, "Mike", "12333", "SALT_333"));
            context.Users.Add(new Models.User(3, "Bob", "TheQuick33#", "SALT_EEE"));
            context.SaveChangesAsync();
            var user1 = context.Users.FirstOrDefault(u => u.Id == 1);
            var user2 = context.Users.FirstOrDefault(u => u.Id == 2);
            var user3 = context.Users.FirstOrDefault(u => u.Id == 3);


            ///Assert
            Assert.AreEqual("Bob",user1.Username);
            Assert.AreEqual("PASSWORD_HASH_32", user1.PasswordHash);
            Assert.AreEqual("SALT_32", user1.SaltBase64String);

            Assert.AreEqual("Mike", user2.Username);
            Assert.AreEqual("12333", user2.PasswordHash);
            Assert.AreEqual("SALT_333", user2.SaltBase64String);


            Assert.AreEqual("Bob", user3.Username);
            Assert.AreEqual("TheQuick33#", user3.PasswordHash);
            Assert.AreEqual("SALT_EEE", user3.SaltBase64String);

            Assert.AreEqual(context.Users.Count(), 3);

        }

            [TestMethod()]
            public async Task RemoveUserTest()
            {

                ///Setup
                var options = GetInMemoryDatabaseOptions();
                var context = new UserContext(options);

                ///Act
                context.Users.Add(new Models.User(1, "Bob", "PASSWORD_HASH_32", "SALT_32"));
                context.Users.Add(new Models.User(2, "Mike", "12333", "SALT_333"));
                context.Users.Add(new Models.User(3, "Bob", "TheQuick33#", "SALT_EEE"));
                await context.SaveChangesAsync();
                var user2 =await context.Users.FirstOrDefaultAsync(u => u.Id == 2);
                Assert.IsNotNull(user2);
                context.Users.Remove(user2);
                await context.SaveChangesAsync();

                ///Assert
                Assert.AreEqual(2,context.Users.Count());
               

            }


    }
}
