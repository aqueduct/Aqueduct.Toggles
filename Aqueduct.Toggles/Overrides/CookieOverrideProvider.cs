using System;
using System.Collections.Generic;
using System.Web;
using Aqueduct.Toggles.Helpers;

namespace Aqueduct.Toggles.Overrides
{
    public class CookieOverrideProvider : IOverrideProvider
    {
        private readonly HttpContextBase _context;
        internal const string CookieName = "Aqueduct.Toggles";

        public string Name => "CookieProvider";

        public CookieOverrideProvider(HttpContextBase context)
        {
            _context = context;
        }

        public CookieOverrideProvider()
        {
        }

        public Dictionary<string, bool> GetOverrides()
        {
            var context = GetCurrentContext();

            var dictionary = new Dictionary<string, bool>();

            if (context == null)
                return dictionary;

            var request = context.Request;

            if (context.Items[CookieName] != null)
            {
                return context.Items[CookieName] as Dictionary<string, bool>;
            }

            var cookie = request.Cookies[CookieName];
            if (cookie != null)
            {
                try
                {
                    var decryptedValue = cookie.Value.Decrypt();
                    var features = decryptedValue.Deserialize<Dictionary<string, bool>>();
                    context.Items[CookieName] = features;

                    return features;
                }
                catch (Exception)
                {
                    return dictionary;
                }
            }
            return dictionary;
        }

        private HttpContextBase GetCurrentContext()
        {
            var context = _context;
            if (context == null && HttpContext.Current != null)
                context = new HttpContextWrapper(HttpContext.Current);
            return context;
        }

        public void SetOverrides(Dictionary<string, bool> overrides)
        {
            var context = GetCurrentContext();

            var cookie = new HttpCookie(CookieName, overrides.Serialize().Encrypt()) { HttpOnly = true };
            context.Response.Cookies.Add(cookie);
        }
    }
}
