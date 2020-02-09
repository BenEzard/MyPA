using System;

namespace MyPA.Code.Util
{
    public static class StringMethods
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
