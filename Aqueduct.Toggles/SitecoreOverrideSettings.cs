namespace Aqueduct.Toggles
{
    public class SitecoreOverrideSettings
    {
        public bool Enabled { get;  }
        public string Path { get;  }
        public string TemplateName { get;  }

        public SitecoreOverrideSettings(bool enabled, string path, string templateName)
        {
            Enabled = enabled;
            Path = path;
            TemplateName = templateName;
        }
    }
}