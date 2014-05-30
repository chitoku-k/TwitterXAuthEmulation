using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterXAuthEmulation
{
    public class Program
    {
        static void Main(string[] args)
        {
            StartTest();
            Console.ReadLine();
        }

        private static void StartTest()
        {
            Console.Clear();
            Console.WriteLine("=== テスト開始 ===\n");
            string name = XAuthEmulation.GetInput("ユーザー名");
            string password = XAuthEmulation.GetSecureInput("パスワード");
            
            Console.WriteLine("\n\nrequest_token の取得中...");
            var session = OAuth.AuthorizeAsync("(YOUR_CONSUMER_KEY)", "(YOUR_CONSUMER_SECRET)");
            try
            {
                session.Wait();
            }
            catch (AggregateException)
            {
                WriteErrorMessage(session.Exception.InnerException.Message);
                StartTest();
                return;
            }
            Console.WriteLine("request_token: " + session.Result.RequestToken);

            Console.WriteLine("\nPIN の取得中...");
            var verifier = XAuthEmulation.GetVerifier(session.Result.RequestToken, name, password);
            try
            {
                verifier.Wait();
            }
            catch (AggregateException)
            {
                WriteErrorMessage(verifier.Exception.InnerException.Message);
                StartTest();
                return;
            }
            Console.WriteLine("PIN: " + verifier.Result);

            Console.WriteLine("\naccess_token の取得中...");
            Console.WriteLine("access_token: " + session.Result.GetTokensAsync(verifier.Result).Result.AccessToken);

            Console.WriteLine("\n=== 完了 ===");
        }

        private static void WriteErrorMessage(string message)
        {
            Console.WriteLine("\n=== エラー ===");
            Console.WriteLine(message);
            Console.WriteLine("何かキーを押してください...");
            Console.ReadLine();
        }
    }
}
