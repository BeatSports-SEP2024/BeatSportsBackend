using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Infrastructure.Common;
public class GenerateRandomAlphanumericExtensions
{
    private static Random random = new Random();

    public static string GenerateRandomAlphanumeric(int length)
    {
        const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringBuilder = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            var byteBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                rng.GetBytes(byteBuffer);
                var num = BitConverter.ToUInt32(byteBuffer, 0);
                stringBuilder.Append(validCharacters[(int)(num % (uint)validCharacters.Length)]);
            }
        }
        return stringBuilder.ToString();
    }
}