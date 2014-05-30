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

public class TwitterXAuthEmulation
{
    public static string GetAuthenticityToken(string consumerKey, string consumerSecret, string requestToken)
    {
        var authenticityTokenPattern = @"<input name=""authenticity_token"" type=""hidden"" value=""(\w+)"" />";
        var codePattern = @"<code>(\d+)</code>";

        var url = "https://api.twitter.com/oauth/authorize?oauth_token=" + requestToken + "&force_login=1";
        var wc = new WebClient { Encoding = Encoding.UTF8 };
        var response = wc.DownloadString(url);
        return Regex.Match(response, authenticityTokenPattern).Groups[1].Value;
    }

    public static AccessToken GetAccessToken(string authenticityToken, string screenName = null, string password = null)
    {
        screenName = screenName ?? GetInput("Name");
        password = password ?? GetSecureInput("Password");

        var data = new NameValueCollection
        { 
            { "authenticity_token", authenticityToken },
            { "oauth_token", requestToken.Token },
            { "force_login", "1" },
            { "session[username_or_email]", screenName },
            { "session[password]", password }
        };

        var result = wc.UploadValues("https://api.twitter.com/oauth/authorize", data);
        response = Encoding.UTF8.GetString(result);

        var verifier = Regex.Match(response, codePattern).Groups[1].Value;
        if (verifier.Length == 0)
        {
            var errorIndex = response.IndexOf("class=\"error notice\"");
            var startTagIndex = response.IndexOf("<p>", errorIndex) + "<p>".Length;
            var endTagIndex = response.IndexOf("</p>", startTagIndex);
            var content = response.Substring(startTagIndex, endTagIndex - startTagIndex);

            var linkIndex = content.IndexOf("<a");
            if (linkIndex >= 0)
                content = content.Remove(linkIndex);

            Console.WriteLine(content);
            Console.ReadLine();
            return GetAccessToken(authenticityToken, null, null);
        }
    }

    private static string GetInput(string name)
    {
        Console.Write("{0}: ", name);
        return Console.ReadLine();
    }

    private static string GetSecureInput(string name)
    {
        Console.WriteLine("{0}: ", name);
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

public class AccessToken
{
    public string Token { get; set; }
    public string TokenSecret { get; set; }
}
