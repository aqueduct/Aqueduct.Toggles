using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aqueduct.Toggles.Overrides
{
    [Serializable]
    public class Override
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<string> Languages { get; }
        [DataMember]
        public bool Enabled { get; set; }
        public string ProviderName { get; set; }

        public Override()
        {

        }

        public Override(string name, bool enabled, List<string> languages = null, string provider = "")
        {
            Name = name;
            ProviderName = provider;
            Enabled = enabled;
            Languages = languages ?? new List<string>();
        }
    }
}