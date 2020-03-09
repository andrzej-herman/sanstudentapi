using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace StudentApi.Helpers
{
    public enum RandomCharacter
    {
        Letter = 1,
        Capital = 2,
        Number = 3,
        Special = 4
    }

    public static class RandomPassword
    {
        private static readonly RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
        private static string letters = "abcdefghijklmnoprstuwxyz";
        private static string capitals = "ABCDEFGHIJKLMNPRSTUWXYZ";
        private static string numbers = "0123456789";
        private static string specials = "#@$&%";


        public static string GenerateTemporaryPassword()
        {
            string res = string.Empty;
            while (res.Length < 10)
            {
                var character = GetCharacter();
                switch (character)
                {
                    case RandomCharacter.Letter:
                        res += letters[GetRandom(0, 23)];
                        break;
                    case RandomCharacter.Capital:
                        res += capitals[GetRandom(0, 22)];
                        break;
                    case RandomCharacter.Number:
                        res += numbers[GetRandom(0, 9)];
                        break;
                    case RandomCharacter.Special:
                        res += specials[GetRandom(0, 4)];
                        break;
                }
            }

            return res;
        }

        public static bool IsNumeric(string value)
        {
            bool checkResult = true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value, i))
                {
                    checkResult = false;
                    break;
                }
            }
            return checkResult;
        }

        private static int GetRandom(int minimum, int maximum)
        {
            byte[] randomNumber = new byte[1];
            generator.GetBytes(randomNumber);
            double asciiValueOfRandomChar = Convert.ToDouble(randomNumber[0]);
            double multiplier = Math.Max(0, (asciiValueOfRandomChar / 255d) - 0.00000000001d);
            int range = maximum - minimum + 1;
            double randomVAlueInRange = Math.Floor(multiplier * range);
            return (int)(minimum + randomVAlueInRange);
        }


        private static RandomCharacter GetCharacter()
        {
            RandomCharacter res = RandomCharacter.Letter;
            int rnd = GetRandom(0, 3);
            switch (rnd)
            {
                case 0:
                    res = RandomCharacter.Letter;
                    break;
                case 1:
                    res = RandomCharacter.Capital;
                    break;
                case 2:
                    res = RandomCharacter.Number;
                    break;
                case 3:
                    res = RandomCharacter.Special;
                    break;
                default:
                    res = RandomCharacter.Letter;
                    break;
            }

            return res;
        }


    }
}
