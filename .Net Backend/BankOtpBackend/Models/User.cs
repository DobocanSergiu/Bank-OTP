namespace WebApplication1.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string SaltBase64String { get; set; }



        public User(long id, string username, string passwordHash, string saltBase64String)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            SaltBase64String = saltBase64String;
        }
    }


}

