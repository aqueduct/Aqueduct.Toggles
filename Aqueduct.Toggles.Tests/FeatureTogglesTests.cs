using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aqueduct.Toggles.Overrides;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Aqueduct.Toggles.Tests
{
    [TestFixture]
    public class FeatureTogglesTests
    {
        [Test]
        public void GetOverrideProvider_ReturnsCurrentProvider()
        {
            var provider = Substitute.For<IOverrideProvider>();
            FeatureToggles.SetOverrideProvider(provider);

            var overrides = FeatureToggles.GetOverrideProviders();

            overrides.Should().Contain(provider);
        }

        [TearDown]
        public void TearDown()
        {
            FeatureToggles.SetOverrideProvider(new CookieOverrideProvider());
        }
    }
}
