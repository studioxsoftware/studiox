using System.Threading.Tasks;

namespace StudioX.Threading
{
    public static class StudioXTaskCache
    {
        public static Task CompletedTask { get; } = Task.FromResult(0);
    }
}
