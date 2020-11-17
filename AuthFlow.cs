using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Fume
{
    public static class AuthFlow
    {
        //https://accounts.spotify.com/authorize?client_id=1da271b7266f4723bd37afa0ff58fe83&response_type=code&redirect_uri=http://localhost/&scope=user-read-playback-state%20playlist-modify-public%20playlist-modify-private%20user-read-currently-playing%20user-library-modify%20user-read-playback-position%20playlist-read-private%20user-library-read

        static string redirect = "http://localhost/";

        public static User FromCode(string scode)
        {
            WebRequest req = WebRequest.Create($"https://accounts.spotify.com/api/token");

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            StreamWriter stream = new StreamWriter(req.GetRequestStream());
            stream.Write($"grant_type=authorization_code&code={scode}&redirect_uri={redirect}&client_id=1da271b7266f4723bd37afa0ff58fe83&client_secret=f868e8d7cd3146e285f33ea3859ae41a");
            stream.Flush();
            stream.Close();

            try
            {
                WebResponse res = req.GetResponse();

                StreamReader reader = new StreamReader(res.GetResponseStream());
                JToken data = JToken.Parse(reader.ReadToEnd());
                AuthFlowResponse flowResponse = data.ToObject<AuthFlowResponse>();

                return new User(flowResponse.access_token) { refreshtoken = flowResponse.refresh_token };
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        public class AuthFlowResponse
        {
            public string access_token, token_type, scope, expires_in, refresh_token;
        }
    }
}
