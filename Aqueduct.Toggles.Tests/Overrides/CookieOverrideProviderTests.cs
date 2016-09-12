using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aqueduct.Toggles.Helpers;
using Aqueduct.Toggles.Overrides;
using FluentAssertions;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace Aqueduct.Toggles.Tests.Overrides
{
    [TestFixture]
    public class CookieOverrideProviderTests
    {
        private CookieOverrideProvider _provider;
        private HttpContextBase _context;
        private Dictionary<string, bool> _validOverrides;

        [SetUp]
        public void Setup()
        {
            _context = Substitute.For<HttpContextBase>();
            _provider = new CookieOverrideProvider(_context);
            _validOverrides = new Dictionary<string, bool> { { "test", true } };
        }

        [Test]
        public void GetOverrides_GivenOverridesInContextItems_ReturnsOverrides()
        {
            var items = new Dictionary<string, object>
                        {
                            {CookieOverrideProvider.CookieName, _validOverrides}
                        };
            _context.Items.Returns(items);

            var overrides = _provider.GetOverrides();

            ValidateOverrides(overrides);
        }

        [Test]
        public void GetOverrides_GivenBadCookie_ReturnsEmptyOverrides()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Cookies.Returns(new HttpCookieCollection
                                    {
                                        new HttpCookie(CookieOverrideProvider.CookieName, "whoops")
                                    });
            _context.Request.Returns(request);

            var overrides = _provider.GetOverrides();

            ValidateEmptyOverrides(overrides);
        }

        [Test]
        public void GetOverrides_GivenGoodCookie_ReturnsOverrides()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Cookies.Returns(new HttpCookieCollection
                                    {
                                        new HttpCookie(CookieOverrideProvider.CookieName, _validOverrides.Serialize().Encrypt())
                                    });
            _context.Request.Returns(request);

            var overrides = _provider.GetOverrides();

            ValidateOverrides(overrides);
        }

        [Test]
        public void GetOverrides_GivenNoCookie_ReturnsEmptyOverrides()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Cookies.Returns(new HttpCookieCollection());
            _context.Request.Returns(request);

            var overrides = _provider.GetOverrides();

            ValidateEmptyOverrides(overrides);
        }

        [Test]
        public void SetOverrides_SetsCookie()
        {
            var response = Substitute.For<HttpResponseBase>();
            var cookies = new HttpCookieCollection();
            response.Cookies.Returns(cookies);
            _context.Response.Returns(response);

            _provider.SetOverrides(_validOverrides);

            cookies.Should().HaveCount(1);
            var cookie = cookies[CookieOverrideProvider.CookieName];
            cookie.Should().NotBeNull();
            var overrides = cookie.Value.Decrypt().Deserialize<Dictionary<string, bool>>();
            ValidateOverrides(overrides);
        }


        private static void ValidateOverrides(IDictionary<string, bool> overrides)
        {
            overrides.Should().ContainKey("test");
            overrides["test"].Should().BeTrue();
        }

        private static void ValidateEmptyOverrides(Dictionary<string, bool> overrides)
        {
            overrides.Should().NotBeNull();
            overrides.Should().BeEmpty();
        }
    }
}
