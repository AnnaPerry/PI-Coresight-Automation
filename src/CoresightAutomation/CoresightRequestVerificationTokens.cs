using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    public class CoresightRequestVerificationTokens
    {
        public CoresightRequestVerificationTokens(string hiddenInputToken = null, CookieContainer cookieContainer = null)
        {
            CookieContainer = cookieContainer ?? new CookieContainer();
            HiddenInputToken = hiddenInputToken;
        }
        public string HiddenInputToken { get; set; }
        public CookieContainer CookieContainer { get; set; }
    }
}
