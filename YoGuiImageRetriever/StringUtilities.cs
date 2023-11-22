using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoGuiImageRetriever
{
    public static class StringUtilities
    {
        public static string FormatText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string s = input.Replace("_", " ");
            s = s.Replace("-", " ");

            string output = string.Empty;
            foreach (char c in s)
            {
                if (char.IsLetter(c) || c == ' ')
                {
                    output += c;
                }
            }

            output = output.Trim();
            output = output.ToLower();
            output = ReplaceWhile(output, "  ", " ");
            output = output.Replace(" ", "-");

            return output.Trim();
        }

        private static string ReplaceWhile(string input, string stringToLook, string replacementString)
        {
            string s = input;
            while (s.Contains(stringToLook))
            {
                s = s.Replace(stringToLook, replacementString);
            }
            return s;
        }
    }
}
