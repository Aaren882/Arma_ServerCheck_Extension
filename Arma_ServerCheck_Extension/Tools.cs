using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;

namespace Arma_ServerCheck_Extension
{
    internal class Tools
    {
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string ExtFilePath = Path.Combine(AssemblyPath, "Arma_ServerCheck_Extension");
        private static readonly string LogFilePath = Path.Combine(ExtFilePath, "logs");
        private static readonly string LogFileName = Path.Combine(LogFilePath, $"{DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss")}.Arma_ServerCheck_Extension.log");

        internal static void Logger(Exception e = null, string s = "", bool loop = false)
        {
            try
            {
                if (!Directory.Exists(ExtFilePath))
                    Directory.CreateDirectory(ExtFilePath);
                if (!Directory.Exists(LogFilePath))
                    Directory.CreateDirectory(LogFilePath);

                using (StreamWriter file = new StreamWriter(@LogFileName, true))
                {
                    if (e != null)
                        s = $"{e}";
                    if (s.Length > 0)
                        file.WriteLine($"{DateTime.Now.ToString("T")} - {s}");
                }
            }
            catch (Exception i)
            {
                if (!loop)
                    Logger(i, null, true);
            };
        }

        internal static int[] StringToCode32(string str)
        {
            int[] code32 = new int[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                // Get the Unicode code point of each character in the string
                code32[i] = char.ConvertToUtf32(str, i);

                // If the character is a surrogate pair, skip the next character
                if (char.IsHighSurrogate(str, i))
                {
                    i++;
                }
            }

            return code32;
        }
    }
}
