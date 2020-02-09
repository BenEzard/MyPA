using System.Collections.Generic;

namespace MyPA.Code.UI.Util
{
    public static class CollectionMethods
    {
        /// <summary>
        /// Generates an incremental list of strings between start and end, by increment.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static List<string> GenerateIncrementalStringList(int start, int end, int increment)
        {
            List<string> rValue = new List<string>();

            int i = start;
            while (i <= end)
            {
                string asStr = i.ToString().PadLeft(2, '0');
                i += increment;
                rValue.Add(asStr);
            }
            return rValue;
        }
    }
}
