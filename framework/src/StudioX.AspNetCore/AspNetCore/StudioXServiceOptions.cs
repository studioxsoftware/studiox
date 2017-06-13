using StudioX.Dependency;
using StudioX.PlugIns;

namespace StudioX.AspNetCore
{
    public class StudioXServiceOptions
    {
        public IIocManager IocManager { get; set; }

        public PlugInSourceList PlugInSources { get; }

        public StudioXServiceOptions()
        {
            PlugInSources = new PlugInSourceList();
        }
    }
}