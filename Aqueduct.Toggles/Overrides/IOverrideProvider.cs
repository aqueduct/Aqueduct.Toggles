using System.Collections.Generic;

namespace Aqueduct.Toggles.Overrides
{
    public interface IOverrideProvider
    {
        string Name { get; }
        Dictionary<string, bool> GetOverrides();
        void SetOverrides(Dictionary<string, bool> overrides);
    }
}