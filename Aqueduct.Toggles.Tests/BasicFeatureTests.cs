using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Aqueduct.Toggles.Overrides;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace Aqueduct.Toggles.Tests
{
    [TestFixture]
    public class BasicFeatureTests
    {
        [SetUp]
        public void Setup()
        {
            SetupOverrides(new Dictionary<string, bool>
                           {
                               {"featureenabledbutoverridden", false},
                               {"featuredisabledbutoverridden", true},
                               {"featuremissingbutoverridden", true}
                           });
        }

        private static void SetupOverrides(Dictionary<string, bool> dictionary)
        {
            var overrides = new Mock<IOverrideProvider>();
            overrides.Setup(x => x.Name).Returns("MockProvider");
            overrides.Setup(x => x.GetOverrides()).Returns(dictionary);
            FeatureToggles.SetOverrideProvider(overrides.Object);
        }

        [TestCase("featureenabled", true)]
        [TestCase("featuredisabled", false)]
        [TestCase("featuremissing", false)]
        [TestCase("featureenabledbutoverridden", false)]
        [TestCase("featuredisabledbutoverridden", true)]
        [TestCase("featuremissingbutoverridden", true)]
        public void ReadsBasicFeaturesFromConfigCorrectly(string feature, bool expected)
        {
            Assert.AreEqual(expected, FeatureToggles.IsEnabled(feature));
        }

        [Test]
        public void GetCssClassesForFeatures_GivenLanguage_ReturnsCorrectString()
        {
            var featuresForClass = FeatureToggles.GetCssClassesForFeatures("current");
            Assert.IsTrue(featuresForClass.Contains(" no-feat-enabledbutwronglanguage"));
            Assert.IsTrue(featuresForClass.Contains(" feat-enabledforcurrentlanguage"));
            Assert.IsTrue(featuresForClass.Contains(" no-feat-featuredisabled"));
            Assert.IsTrue(featuresForClass.Contains("feat-featureenabled"));
        }

        [Test]
        public void GetCssClassesForFeatures_GivenNoLanguage_ReturnsCorrectString()
        {
            var featuresForClass = FeatureToggles.GetCssClassesForFeatures();
            Assert.IsTrue(featuresForClass.Contains(" feat-enabledbutwronglanguage"));
            Assert.IsTrue(featuresForClass.Contains(" feat-enabledforcurrentlanguage"));
            Assert.IsTrue(featuresForClass.Contains(" no-feat-featuredisabled"));
            Assert.IsTrue(featuresForClass.Contains("feat-featureenabled"));
        }

        [Test]
        public void GetAllFeatures_GivenFeaturesInConfig_ReturnsElevenFeatureToggles()
        {
            var features = FeatureToggles.GetAllFeatures();

            Assert.AreEqual(13, features.Count());
        }

        [Test]
        public void GetAllEnabledFeatures_GivenFeaturesInConfig_ReturnsElevenFeatureToggles()
        {
            var features = FeatureToggles.GetAllEnabledFeatures();

            Assert.AreEqual(9, features.Count());
        }

        [Test]
        public void GetAllFeatures_GivenFeatureEnabledConfig_ReturnsEnabledFeature()
        {
            var feature = FeatureToggles.GetAllFeatures().FirstOrDefault(x => x.Name == "featureenabled");

            Assert.IsNotNull(feature);
            Assert.AreEqual("featureenabled", feature.Name);
            Assert.AreEqual(true, feature.Enabled);
        }

        [Test]
        public void GetAllFeatures_ReturnsFeaturesDescriptionAndStepsAlongWithTheFeatures()
        {
            //Arrange
            var feature = FeatureToggles.GetAllFeatures().First();

            //Assert
            Assert.IsNotNull(feature);
            Assert.AreEqual("Short description", feature.ShortDescription);
            Assert.AreEqual("<li>Step1</li>", feature.Requirements);
        }

        [Test]
        public void GetAllFeatures_GivenFeatureEnabledConfigButOverriddenByTheUser_ReturnsDisabledFeature()
        {
            SetupOverrides(new Dictionary<string, bool> { { "featureenabledTest", false } });

            var enabled = FeatureToggles.IsEnabled("featureenabledTest");
            Assert.False(enabled);
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}