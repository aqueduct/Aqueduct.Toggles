using System;
using System.IO;
using Aqueduct.Toggles.Sitecore.Overrides;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace Aqueduct.Toggles.Sitecore.EventHandlers
{
    public class FeatureEventHandler
    {
        //Delivery DBs
        public void OnPublishEndRemoteHandler(object sender, EventArgs args)
        {
            var remoteEventArgs = args as PublishEndRemoteEventArgs;
            if (remoteEventArgs != null)
            {
                Database database = null;
                try
                {
                    database = Database.GetDatabase(remoteEventArgs.TargetDatabaseName);
                }
                catch (Exception)
                {
                }
                //Hardcoded to Web
                if (database == null)
                {
                    try
                    {
                        database = Database.GetDatabase("web");
                    }
                    catch (Exception)
                    {
                    }
                }

                if (database != null)
                {
                    var rootItem = database.GetItem(new ID(remoteEventArgs.RootItemId));
                    if (rootItem != null)
                    {
                        if ((remoteEventArgs.Deep &&
                             FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath
                                 .Contains(
                                     rootItem.Paths.Path)) ||
                            rootItem.Paths.Path.Contains(
                                FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath))
                        {
                            SitecoreOverrideProvider.Instance.RefreshSitecoreOverrides(
                                database.Name);
                        }
                    }
                }
            }
        }

        //Master DB
        public void OnItemSavedHandler(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;

            if (item != null && item.Paths.Path.Contains(FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath)
                && item.Database.Name.Equals("master"))
            {
                SitecoreOverrideProvider.Instance.RefreshSitecoreOverrides(item.Database.Name);
            }
        }

        //Master DB
        public void OnItemDeletedHandler(object sender, EventArgs args)
        {
            var sitecoreID = Event.ExtractParameter(args, 1) as ID;
            var item = Event.ExtractParameter(args, 0) as Item;

            var rootItem = item?.Database.GetItem(
                FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath);

            if (rootItem != null && sitecoreID == rootItem.ID)
            {
                SitecoreOverrideProvider.Instance.RefreshSitecoreOverrides(item.Database.Name);
            }
        }

        //Master DB
        public void OnItemDeletedRemoteHandler(object sender, EventArgs args)
        {
            ItemDeletedRemoteEventArgs deleteEvent = (ItemDeletedRemoteEventArgs)args;

            var rootItem = deleteEvent.Item?.Database.GetItem(
                FeatureToggles.Configuration.FeatureToggleConfigurationSection.SitecoreOverridesPath);

            if (rootItem != null && deleteEvent.ParentId == rootItem.ID)
            {
                SitecoreOverrideProvider.Instance.RefreshSitecoreOverrides(deleteEvent.Item.Database.Name);
            }
        }
    }
}
