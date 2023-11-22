using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoGuiImageRetriever
{
    public static class StringUtilities
    {
        public static string FormatText(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return inputString;

            // Remove all non-letter characters using regex
            string lettersOnly = Regex.Replace(inputString, @"[^a-zA-Z ]", "");

            // Capitalize the first letter of each word
            string[] words = lettersOnly.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    char[] letters = words[i].ToCharArray();
                    letters[0] = char.ToUpper(letters[0]);
                    words[i] = new string(letters);
                }
            }

            // Join the words back into a string
            string formattedString = string.Join(" ", words);

            return formattedString;
        }
    }
}
