namespace Server;

public class UserManager
{
    private static List<User> users = new List<User>();

    public static void AddUser(User user)
    {
        users.Add(user);
    }

    public static User? GetUser(string username)
    {
        return users.FirstOrDefault(u => u.Username == username);
    }

    public static bool ValidateUser(string username, string password)
    {
        var user = GetUser(username);
        return user != null && user.Password == password;
    }
}