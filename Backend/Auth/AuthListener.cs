using System;
using System.Net;

namespace Spotter
{
    public static class AuthListener
    {
        #region Fields

        private static HttpListener listener;

        #endregion Fields

        #region Methods

        private static async void Request(IAsyncResult result)
        {
            HttpListenerContext listenerContext = listener.EndGetContext(result);
            listener.BeginGetContext(Request, null);

            listenerContext.Response.StatusCode = 401;

            string code = listenerContext.Request.QueryString["code"];
            if (code != null)
            {
                User u = AuthFlow.FromCode(code);
                if (u != null)
                {
                    Memory.Add(u);
                    listenerContext.Response.StatusCode = 200;
                }
            }

            listenerContext.Response.Close();
        }

        public static void Start(int port = 3000, bool external = false)
        {
            listener = new HttpListener();
            if (external) listener.Prefixes.Add($"http://+:{port}/");
            else listener.Prefixes.Add($"http://localhost:{port}/");

            listener.Start();
            listener.BeginGetContext(Request, null);
        }

        #endregion Methods
    }
}
