using System.Linq;

namespace CrazyKTV_SongMgr
{
    public static class StringExtension
    {
        public static bool ContainsAll(this string source, params string[] values)
        {
            return values.All(x => source.Contains(x));
        }
    }
}
