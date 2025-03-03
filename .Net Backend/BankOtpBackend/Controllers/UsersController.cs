using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using OtpNet;

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        /// Simulates cache where time-based otp codes are stored
        private static List<OtpObject> otpCodesCache = new List<OtpObject>();


        public UsersController(UserContext context)
        {
            _context = context;
        }


        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCredentials credentials)
        {

            ///Check to make sure username and password is given
            if (credentials == null || string.IsNullOrEmpty(credentials.Username) ||
       string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }


            Random random = new Random();
            long id;
            ///Check to see if usernames are unique
            if (await _context.Users.AnyAsync(u => u.Username == credentials.Username))
            {
                return Conflict(new { message = "Username is already taken" });
            }

            ///Check to see if password is complex enough
            if(isValidPassword(credentials.Password)==false)
            {
                return BadRequest(new { message = "Password is not complex enough" });
            }

         
            do
            {
                id = random.Next(1000000000, 2000000000); /// Generate 10-digit ID
            }
            while (await _context.Users.AnyAsync(u => u.Id == id)); /// Check if the ID exists

            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);///Generating salt;
            string SaltBase64String = Convert.ToBase64String(salt);

            string PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(  ///Generating password hash;
            password: credentials.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

            User newUser = new User(id, credentials.Username, PasswordHash, SaltBase64String);
            _context.Users.AddAsync(newUser);
            _context.SaveChangesAsync();
            return StatusCode(201, new { message = "User created successfully" });
        }

        static bool isValidPassword(string password)
        {
            string specialCharacters = "!@#$%^&*()_+=~`';:<>,.";
            string capitalLetters = "QAZWSXEDCRFVTGBYHNUJMIKOLP";
            string numbers = "1234567890";

            if(password.Length<8)
            {
                return false;
            }
            int i;
            bool hasCapitalLetter = false;
            bool hasNumbers = false;
            bool hasSpecialCharacter = false;
            for(i=0;i<password.Length;i++)
            {
                if (specialCharacters.Contains(password[i]))
                {
                    hasSpecialCharacter = true;
                }

                if (numbers.Contains(password[i]))
                {
                    hasNumbers = true;
                }

                if (capitalLetters.Contains(password[i]))
                {
                    hasCapitalLetter = true;
                }
            }
            if(hasSpecialCharacter && hasNumbers && hasCapitalLetter)
            {
                return true;
            }
            return false;

        }




        [HttpPost("loginUser")]
        public async Task<ActionResult<User>> LoginUser([FromBody] UserCredentials credentials)
        {

            if (credentials == null || string.IsNullOrEmpty(credentials.Username) ||
                string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == credentials.Username);

            if (user == null)
            {
                return NotFound(new { message = "User not found." }); // If the user doesn't exist
            }

            var passwordSaltString = user.SaltBase64String; //Getting salt of user
            byte[] saltByteArray = Convert.FromBase64String(passwordSaltString);

            string inputPasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(  ///Generating password hash;
            password: credentials.Password,
            salt: saltByteArray,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

            if (inputPasswordHash != user.PasswordHash)
            {
                return Unauthorized(new { message = "Invalid Password" });
            }




            return Ok(new { message = "Login Succesfull" });
        }

        [HttpPost("requestCode")]
        public async Task<IActionResult> RequestCode([FromBody] OtpCodeRequest codeRequest)
        {


            var user = await _context.Users.FirstOrDefaultAsync(i => i.Username == codeRequest.Username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            otpCodesCache.RemoveAll(item => item.Username == codeRequest.Username);


            byte[] secretKey = new byte[20];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(secretKey);
            var totp = new Totp(secretKey,step:30,mode:OtpHashMode.Sha256,totpSize:6);
            string current_code = totp.ComputeTotp();
            OtpObject otpObject = new OtpObject(current_code,codeRequest.Username,DateTime.UtcNow.AddSeconds(30),30,Base32Encoding.ToString(secretKey),5,0);
            otpCodesCache.Add(otpObject);
           


            return Ok(new {owner=codeRequest.Username ,code = current_code, time = otpObject.LifeTime});

                    
        }
        [HttpPost("validateCode")]
        public async Task<IActionResult> ValidateCode([FromBody] OtpCodeValidationRequest codeValidationRequest)
        {
            /// Finding user
            var user = await _context.Users.FirstOrDefaultAsync(i => i.Username == codeValidationRequest.Username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            /// Try to find the cache entry for the user
            var cacheEntry = otpCodesCache.FirstOrDefault(i => i.Username == codeValidationRequest.Username);

            /// If cache entry does not exist, the user has not requested an OTP code yet
            if (cacheEntry == null)
            {
                return NotFound(new { message = "Code does not exist" });
            }

            /// Increment attempts made and check if the max attempts are exceeded
            cacheEntry.AttemptsMade++;
            if (cacheEntry.AttemptsMade > cacheEntry.MaxAttempts)
            {
                otpCodesCache.Remove(cacheEntry);
                return BadRequest(new { message = "Max number of attempts used, try again" });
            }

            /// Checking if the input code matches the cached OTP code
            if (cacheEntry.Code != codeValidationRequest.Code)
            {
                return BadRequest(new { message = "Incorrect code" });
            }

            /// Check to see if the OTP code is expired
            if (cacheEntry.ExpirationDateTime < DateTime.UtcNow)
            {
                otpCodesCache.Remove(cacheEntry);
                return BadRequest(new { message = "Code has expired" });
            }

            /// If the code is valid and not expired, remove the cache entry and reset the attempts counter
            otpCodesCache.Remove(cacheEntry);
            return Ok(new { message = "Validation successful" });
        }




        public class UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class OtpCodeRequest
        {
            public string Username { get; set; }

        }

        public class OtpCodeValidationRequest
        {
            public string Username { get; set; }
            public string Code { get; set; }

        }

    }
}
