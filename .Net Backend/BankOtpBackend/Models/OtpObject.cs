using Humanizer.DateTimeHumanizeStrategy;

namespace WebApplication1.Models
{
    public class OtpObject
    {

        public OtpObject(string Code, string Username, DateTime ExpirationDateTime, int LifeTime, string SecretKey,int MaxAttempts,int AttemptsMade) { 
        this.Code = Code;
        this.Username = Username;
        this.ExpirationDateTime = ExpirationDateTime;
        this.LifeTime = LifeTime;
        this.SecretKey = SecretKey;
        this.MaxAttempts = MaxAttempts;
        this.AttemptsMade = AttemptsMade;
        }
        public string Code { get; set; }
        public string Username { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public int LifeTime { get; set; } ///How much time (in sec) the code will be valid
        
        public string SecretKey { get; set; }

        public int MaxAttempts { get; set; }

        public int AttemptsMade {get; set; }


    

    }
}
