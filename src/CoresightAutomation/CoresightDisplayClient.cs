using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    /// <summary>
    /// A low-level class for creating PI Coresight displays. 
    /// Performs the actions necessary to persist a new display.
    /// </summary>
    public class CoresightDisplayClient
    {
        public CoresightDisplayClient(Uri coresightBaseUri, ICredentials credentials = null)
        {
            _coresightBaseUri = coresightBaseUri;
            _credentials = null;
            _tokens = GetRequestVerificationTokensAsync().GetAwaiter().GetResult();
            _requestId = GetNewRequestIdAsync().GetAwaiter().GetResult();
            DisplayWrapper = new DisplayWrapper(_requestId);
        }

        /// <summary>
        /// Gets request verification tokens from the server
        /// </summary>
        /// <returns>An object with a field value and a cookie collection</returns>
        private async Task<CoresightRequestVerificationTokens> GetRequestVerificationTokensAsync()
        {
            CoresightRequestVerificationTokens tokens = new CoresightRequestVerificationTokens();
            using (var client = new CoresightHttpClient(_coresightBaseUri, tokens, _credentials))
            {
                Console.WriteLine("Getting Coresight verification tokens.");
                HttpResponseMessage response = await client.HttpClient.GetAsync("");
                Console.WriteLine("Response: {0} - {1}", response.StatusCode, response.ReasonPhrase);

                string content = await response.Content.ReadAsStringAsync();

                Regex tokenFinder = new Regex(@"(?:window\.Coresight\.RequestVerificationToken)\s*?=\s*'([\w:-]+?)'");
                tokens.HiddenInputToken = tokenFinder.Match(content).Groups[1].Captures[0].Value;

                return tokens;
            }
        }


        /// <summary>
        /// Begins a new display, tied to a unique RequestId on the server.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetNewRequestIdAsync()
        {
            using (var coresightClient = new CoresightHttpClient(_coresightBaseUri, _tokens, _credentials))
            {
                string newDisplayJson = await coresightClient.HttpClient.GetStringAsync("Displays/NewDisplay");
                DisplayInfo newDisplay = JsonConvert.DeserializeObject<DisplayInfo>(newDisplayJson);
                return newDisplay.RequestId;
            }
        }

        public async Task<DisplayRevision> SaveAsync()
        {
            using (var coresightClient = new CoresightHttpClient(_coresightBaseUri, _tokens, _credentials))
            {
                string displayWrapperJson = JsonConvert.SerializeObject(DisplayWrapper);
                StringContent content = new StringContent(displayWrapperJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await coresightClient.HttpClient.PostAsync("Displays/SaveDisplay", content);
                response.EnsureSuccessStatusCode();
                string responseContentJson = await response.Content.ReadAsStringAsync();
                DisplayRevision displayRevision = JsonConvert.DeserializeObject<DisplayRevision>(responseContentJson);
                return displayRevision;
            }
        }

        public DisplayWrapper DisplayWrapper { get; private set; }

        ICredentials _credentials = null;
        CoresightRequestVerificationTokens _tokens;
        Uri _coresightBaseUri;
        string _requestId;

    }
}
