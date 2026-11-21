public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Role { get; set; }

    public User() { }

    public User(int id, string login, string password, int role)
    {
        Id = id;
        Login = login;
        Password = password;
        Role = role;
    }
}
