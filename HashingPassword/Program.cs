using HashingPassword;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
using System.Text;
using static System.Console;

Prompt();

void Prompt()
{
    Clear();
    WriteLine("[R] Register [L] login");
    
    while (true)
    {
        var input = ReadLine().ToUpper()[0];
        switch (input)
        {
            case 'R': Register(); break;
            case 'L': LogIn(); break;
            default:
                break;
        }
    }
}

void LogIn()
{
    Clear();
    WriteLine("################Login################");
    WriteLine("User Name");
    var name = ReadLine();
    WriteLine("Password");
    var password = ReadLine();
    using AppDataContext context = new AppDataContext();
    var userFound=context.Users.Any(u => u.Name == name);
    if (userFound)
    {
        var loginUser=context.Users.FirstOrDefault(u => u.Name == name);

        if (PasswordHash($"{password}{loginUser.Salt}") == loginUser.Password)
        {
            Clear();
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Login Successful");
            ReadLine();
        }
        else
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Login Failed");
            ReadLine();
        }
    }
}

void Register()
{
    Clear();
    WriteLine("################Register################");
    WriteLine("User Name");
    var name = ReadLine();
    WriteLine("Password");
    var password = ReadLine();
    using AppDataContext context = new AppDataContext();

    var salt = DateTime.Now.ToString();
    var HashedPassword = PasswordHash($"{password}{salt}");
    context.Users.Add(new User() { Name = name, Password = HashedPassword, Salt = salt });
    context.SaveChanges();
    while (true)
    {
        Clear();
        WriteLine("Registration Complete");
        WriteLine("[B] Back");
        if(ReadKey().Key == ConsoleKey.B)
        {
            Prompt();
        }
    }
}

string PasswordHash(string password)
{
    SHA256 hash= SHA256.Create();
    var passwordBytes=Encoding.Default.GetBytes(password);
    var hashedPassword=hash.ComputeHash(passwordBytes);
    return Convert.ToHexString(hashedPassword);

}
