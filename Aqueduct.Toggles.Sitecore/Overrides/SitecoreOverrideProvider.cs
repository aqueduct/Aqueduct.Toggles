using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aqueduct.Toggles.Overrides;
using Aqueduct.Toggles.Sitecore.Overrides;
using Sitecore.Data.Items;
using Sitecore.StringExtensions;
using SC = Sitecore;

namespace Aqueduct.Toggles.Sitecore.Overrides
{
    public class SitecoreOverrideProvider : IOverrideProvider
    {
        public string Name => "Sitecore";

        private static SitecoreOverrideProvider _sitecoreOverrideProvider;

        private static readonly ReaderWriterLockSlim ReadWriteLock = new ReaderWriterLockSlim();
        private Dictionary<string, bool> _sitecoreOverrideDictionary;

        public static SitecoreOverrideProvider Instance
        {
            get
            {
                ReadWriteLock.EnterWriteLock();
                try
                {
                    if (_sitecoreOverrideProvider == null)
                    {
                        _sitecoreOverrideProvider = new SitecoreOverrideProvider();
                        _sitecoreOverrideProvider.RefreshSitecoreOverrides();
                    }

                    return _sitecoreOverrideProvider;
                }
                finally
                {
                    ReadWriteLock.ExitWriteLock();
                }
            }
        }

        public void RefreshSitecoreOverrides(string databaseName = null)
        {
            _sitecoreOverrideDictionary = new Dictionary<string, bool>();

            try
            {
                if (HttpContext.Current != null || !databaseName.IsNullOrEmpty())
                {
                    SC.Data.Database database;

                    if (databaseName.IsNullOrEmpty())
                    {
                        if (SC.Context.Database != null && SC.Context.Database.Name != "core")
                            database = SC.Context.Database;
                        else
                        {
                            database = SC.Data.Database.GetDatabase(FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreFeatureDatabaseDefault);
                        }
                    }
                    else
                    {
                        database = SC.Data.Database.GetDatabase(databaseName);
                    }

                    var featureCollection =
                        database?.GetItem(
                            FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath);

                    if (featureCollection != null)
                    {
                        foreach (Item child in featureCollection.Children)
                        {
                            if (!_sitecoreOverrideDictionary.ContainsKey(child.Fields["Name"].ToString()))
                            {
                                _sitecoreOverrideDictionary.Add(child.Fields["Name"].ToString(),
                                    child.Fields["Enabled"].ToString() == "1");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //handler execption..
                //Write to default sitecore logger.
            }
        }

        public Dictionary<string, bool> GetOverrides()
        {
            return Instance._sitecoreOverrideDictionary;
        }

        public void SetOverrides(Dictionary<string, bool> overrides)
        {
            throw new NotImplementedException("You can only set overrides from the sitecore interface");
        }
    }
}
