using System;

namespace TwitterXAuthEmulation
{
    public class Utilities
    {
        /// <summary>
        /// ユーザーから文字列を入力します。
        /// </summary>
        /// <param name="name">パラメーター名。</param>
        /// <returns>入力された文字列。</returns>
        public static string GetInput(string name)
        {
            Console.Write("{0}: ", name);
            return Console.ReadLine();
        }

        /// <summary>
        /// パスワード形式で、ユーザーから文字列を入力します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns>入力された文字列。</returns>
        public static string GetSecureInput(string name)
        {
            Console.Write("{0}: ", name);
            ConsoleKeyInfo key;

            string secureString = "";
            do
            {
                key = Console.ReadKey(true);

                if (key.KeyChar == '\r')
                {
                    break;
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    secureString += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    secureString = secureString.Remove(secureString.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return secureString;
        }
    }
}
