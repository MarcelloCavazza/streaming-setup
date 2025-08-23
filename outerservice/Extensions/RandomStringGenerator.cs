using System.Text;

namespace outerservice.Extensions;

public static class RandomStringGenerator
{
    private static readonly Random _random = new Random();
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string Generate()
    {
        int length = _random.Next(1, 11); // 1 to 10 chars
        var buffer = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            string letter = _chars[_random.Next(_chars.Length)].ToString();
            if(_random.Next(2) == 1)
            {
                letter = letter.ToLower();
            }

            buffer.Append(letter);
        }
        return buffer.ToString();
    }
}