namespace Server;

public class User
{
    public string Username { get; set; }   
    public string Password { get; set; }
    public bool isLogged { get; set; } = false;
    
    public User(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }
}