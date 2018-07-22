using System;
using System.Linq;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public static class StringExtension
    {
        public static bool ContainsAll(this string source, params string[] values)
        {
            if (values.Count() <= 0) return false;
            return values.All(x => source.Contains(x));
        }

        public static bool ContainsAny(this string source, params string[] values)
        {
            if (values.Count() <= 0) return false;
            return values.Any(x => source.Contains(x));
        }

        public static bool ContainsCount(this string source, int count, params string[] values)
        {
            if (values.Count() <= 0) return false;
            return values.Where(x => source.Contains(x)).Count() == count;
        }
    }

    public static class ControlExtensions
    {
        public static T InvokeIfRequired<T>(this T source, Action<T> action)
            where T : Control
        {
            try
            {
                if (!source.InvokeRequired)
                    action(source);
                else
                    source.Invoke(new Action(() => action(source)));
            }
            catch (Exception ex)
            {
                Console.Write("Error on 'InvokeIfRequired': {0}", ex.Message);
            }
            return source;
        }
    }
}
