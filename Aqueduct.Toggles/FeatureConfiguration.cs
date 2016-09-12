using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Aqueduct.Toggles.Configuration;

namespace Aqueduct.Toggles
{
    public class FeatureConfiguration
    {
        internal static readonly FeatureToggleConfigurationSection FeatureToggleConfiguration = ConfigurationManager.GetSection("featureToggles") as FeatureToggleConfigurationSection;

        private IList<Feature> _features = new List<Feature>();

        public FeatureConfiguration()
        {
            if (FeatureToggleConfiguration == null) throw new ConfigurationErrorsException("Missing featureToggles section in config.");
            LoadFromConfiguration();
        }

        protected void LoadFromConfiguration()
        {
            _features = FeatureToggleConfiguration.Features.Cast<FeatureToggleConfigurationElement>().Select(Feature.FromConfig).ToList();
        }

        public FeatureToggleConfigurationSection FeatureToggleConfigurationSection => FeatureToggleConfiguration;

        public IEnumerable<Feature> AllFeatures => _features;

        public IEnumerable<Feature> EnabledFeatures => _features.Where(IsEnabled);

        public bool IsEnabled(string name)
        {
            return
            IsEnabled(GetFeature(name));
        }

        public bool IsEnabled(Feature feature)
        {
            if (feature == null)
                return false;

            return feature.Enabled;
        }

        public Feature GetFeature(string name)
        {
            return _features.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}