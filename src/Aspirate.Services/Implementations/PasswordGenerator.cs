namespace Aspirate.Services.Implementations;

public class PasswordGenerator : IPasswordGenerator
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!";

    public string Generate(int length = 24)
    {
        var charactersLength = Characters.Length;
        var password = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            password.Append(Characters[RandomNumberGenerator.GetInt32(charactersLength)]);
        }

        return password.ToString();
    }
}
