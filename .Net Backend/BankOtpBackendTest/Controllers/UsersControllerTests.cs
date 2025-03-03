using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;
using static WebApplication1.Controllers.UsersController;

namespace WebApplication1.Tests
{
    [TestClass()]
    public class UsersControllerTests
    {
        private UserContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new UserContext(options);
            return context;
        }

        [TestMethod()]
        public async Task CreateValidUser()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var credentials = new UserCredentials
            {
                Username = "testuser",
                Password = "Test@1234"
            };

            /// Act
            var result = await controller.CreateUser(credentials);

            /// Assert
            var statusCodeResult = (ObjectResult)result;
            Assert.AreEqual(201, statusCodeResult.StatusCode);
            Assert.AreEqual(1, await context.Users.CountAsync());
        }

        [TestMethod()]
        public async Task NullCredentialsTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);

            /// Act
            var result = await controller.CreateUser(null);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod()]
        public async Task MissingUsernameTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var credentials = new UserCredentials
            {
                Username = "",
                Password = "TestPassword322@"
            };

            /// Act
            var result = await controller.CreateUser(credentials);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result;
            Assert.IsTrue(badRequest.Value.ToString().Contains("Username and password are required"));

        }

        [TestMethod()]
        public async Task MissingPasswordTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var credentials = new UserCredentials
            {
                Username = "testuser",
                Password = ""
            };

            /// Act
            var result = await controller.CreateUser(credentials);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result;
            Assert.IsTrue(badRequest.Value.ToString().Contains("Username and password are required"));

        }

        [TestMethod()]
        public async Task DuplicateNameTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);

            /// Adding original user
            var existingUser = new User(1234567890, "testuser", "hashedPassword", "salt");
            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();

            var credentials = new UserCredentials
            {
                Username = "testuser",
                Password = "Test@1234"
            };

            /// Act
            var result = await controller.CreateUser(credentials);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
            var conflictRequest = (ConflictObjectResult)result;
            Assert.IsTrue(conflictRequest.Value.ToString().Contains("Username is already taken"));

        }

        [TestMethod()]
        public async Task SimplePasswordTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var credentials = new UserCredentials
            {
                Username = "testuser",
                Password = "123123" // No capital letters, numbers, or special characters
            };

            /// Act
            var result = await controller.CreateUser(credentials);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result;
            Assert.IsTrue(badRequest.Value.ToString().Contains("Password is not complex enough"));
        }

        [TestMethod()]
        public async Task ValidCredentialsTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);

            /// First create a user
            var createCredentials = new UserCredentials
            {
                Username = "testuser",
                Password = "Th1sisavalidpassword123!@"
            };
            await controller.CreateUser(createCredentials);
            
            /// Act
            var result = await controller.LoginUser(createCredentials);

            /// Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));


        }

        [TestMethod()]
        public async Task NonExistentUserTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var credentials = new UserCredentials
            {
                Username = "nonexistentuser",
                Password = "Test@1234"
            };

            /// Act
            var result = await controller.LoginUser(credentials);

            /// Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));

        }

        [TestMethod]
        public async Task WrongPasswordTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);

            /// First create a user
            var createCredentials = new UserCredentials
            {
                Username = "testuser",
                Password = "Test@1234"
            };
            await controller.CreateUser(createCredentials);
            
            /// Try to login with wrong password
            var loginCredentials = new UserCredentials
            {
                Username = "testuser",
                Password = "WrongPassword@1"
            };

            /// Act
            var result = await controller.LoginUser(loginCredentials);

            /// Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod()]
        public async Task ValidOtpTokenTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            
            /// First create a user
            var createCredentials = new UserCredentials
            {
                Username = "testuser",
                Password = "Th1sisavalidpassword123!@"
            };
            await controller.CreateUser(createCredentials);

            /// Creating otp request
            var codeRequest = new OtpCodeRequest();
            codeRequest.Username = "testuser";

            

            ///Act
            var result = controller.RequestCode(codeRequest);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));



        }

        [TestMethod()]
        public async Task RequestTokkenOfFalseUserTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var codeRequest = new OtpCodeRequest
            {
                Username = "nonexistentuser"
            };

            /// Act
            var result = await controller.RequestCode(codeRequest);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }


        [TestMethod()]
        public async Task ValidateCodeNonExistentUserTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);
            var validationRequest = new OtpCodeValidationRequest
            {
                Username = "nonexistentuser",
                Code = "123456"
            };

            /// Act
            var result = await controller.ValidateCode(validationRequest);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod()]
        public async Task ValidateCodeInvalidCodeTest()
        {
            /// Setup
            var context = GetMockContext();
            var controller = new UsersController(context);

            /// Create a user
            var user = new User(1234567890, "testuser", "hashedPassword", "salt");
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            /// Request a code
            var codeRequest = new OtpCodeRequest { Username = "testuser" };
            await controller.RequestCode(codeRequest);

            var validationRequest = new OtpCodeValidationRequest
            {
                Username = "testuser",
                Code = "999999" // Invalid code
            };

            // Act
            var result = await controller.ValidateCode(validationRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        
    }
}