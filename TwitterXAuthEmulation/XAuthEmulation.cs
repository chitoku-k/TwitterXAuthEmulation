/*
 * TwitterXAuthEmulation - https://github.com/chitoku-k/TwitterXAuthEmulation
 * -----------------------------------------------------------------------
 * Licensed under the MIT license.
 *
 * Copyright (c) 2014 Chitoku
 */

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitterXAuthEmulation
{
    public class XAuthEmulation
    {
        static WebClient _client = new WebClient();

        const string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        const string AuthenticityTokenPattern = @"<input name=""authenticity_token"" type=""hidden"" value=""(\w+)"" />";
        const string CodePattern = @"<code>(\d+)</code>";
        const string ErrorPattern = @"<div class=""error notice"".*?>.*?<p>(.*?)(?:<a.*?)?<\/p>.*?<\/div>";

        /// <summary>
        /// 非同期操作としてリクエスト トークンおよびユーザー名とパスワードを使用して、PIN コードを取得します。
        /// </summary>
        /// <param name="requestToken">トークン取得に使用するリクエスト トークン。</param>
        /// <param name="screenName">トークン取得に使用するユーザー名。</param>
        /// <param name="password">トークン取得に使用するパスワード。</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.WebException"/>
        /// <returns>PIN コードを表す <see cref="System.String"/> を返します。</returns>
        public static async Task<string> GetVerifier(string requestToken, string screenName, string password)
        {
            if (requestToken == null)
                throw new ArgumentNullException("requestToken");

            if (screenName == null)
                throw new ArgumentNullException("screenName");

            if (password == null)
                throw new ArgumentNullException("password");

            string token = await GetAuthenticityToken(requestToken);
            return await GetVerifier(token, requestToken, screenName, password);
        }

        private static async Task<string> GetAuthenticityToken(string requestToken)
        {
            string response = await _client.DownloadStringTaskAsync(AuthorizeUri + "?oauth_token=" + requestToken);
            return Regex.Match(response, AuthenticityTokenPattern).Groups[1].Value;
        }

        private static async Task<string> GetVerifier(string authenticityToken, string requestToken, string screenName, string password)
        {
            var data = new NameValueCollection
            { 
                { "authenticity_token", authenticityToken },
                { "oauth_token", requestToken },
                { "force_login", "1" },
                { "session[username_or_email]", screenName },
                { "session[password]", password }
            };

            var result = await _client.UploadValuesTaskAsync("https://api.twitter.com/oauth/authorize", data);
            string response = Encoding.UTF8.GetString(result);

            var verifier = Regex.Match(response, CodePattern).Groups[1];
            if (!verifier.Success)
                throw new WebException(Regex.Match(response, ErrorPattern, RegexOptions.Singleline).Groups[1].Value, WebExceptionStatus.Success);

            return verifier.Value;
        }

        public static string GetInput(string name)
        {
            Console.Write("{0}: ", name);
            return Console.ReadLine();
        }

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
