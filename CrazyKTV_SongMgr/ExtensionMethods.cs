using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
