using StudioX.Collections;
using StudioX.Modules;

namespace StudioX.AspNetCore.TestBase
{
    public class TestOptions
    {
        public ITypeList<StudioXModule> Modules { get; private set; }

        public TestOptions()
        {
            Modules = new TypeList<StudioXModule>();
        }
    }
}