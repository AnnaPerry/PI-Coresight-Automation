using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    public class CoresightHttpClient : IDisposable
    {
        public CoresightHttpClient(Uri coresightBaseUri, CoresightRequestVerificationTokens tokens, ICredentials credentials = null)
        {
            _coresightBaseUri = coresightBaseUri;
            _tokens = tokens;

            _clientHandler = credentials != null ?
                                new HttpClientHandler()
                                {
                                    CookieContainer = _tokens.CookieContainer,
                                    Credentials = credentials,
                                    PreAuthenticate = true
                                } :
                                new HttpClientHandler()
                                {
                                    CookieContainer = _tokens.CookieContainer,
                                    UseDefaultCredentials = true,
                                    PreAuthenticate = true
                                };
            HttpClient = new HttpClient(_clientHandler)
            {
                BaseAddress = _coresightBaseUri
            };

            HttpClient.DefaultRequestHeaders.Add("RequestVerificationToken", _tokens.HiddenInputToken);
            HttpClient.DefaultRequestHeaders.Add("Referer", _coresightBaseUri.OriginalString);
            HttpClient.DefaultRequestHeaders.Add("Host", _coresightBaseUri.Host);
        }

        public HttpClient HttpClient { get; private set; }

        Uri _coresightBaseUri;
        HttpClientHandler _clientHandler;
        CoresightRequestVerificationTokens _tokens;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpClient.Dispose();
                    _clientHandler.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
