using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPA.Code.UI.Util
{
    public static class DateMethods
    {
        /// <summary>
        /// Calculates a description for the difference between 2 dates.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="shortVersion"></param>
        /// <returns></returns>
        public static string GenerateDateDifferenceLabel(DateTime date1, DateTime date2, bool shortVersion)
        {
            string rValue = "";
            bool needComma = false;
            bool valueOutputted = false;    // This is set to true if shortVersion==true and some time-related value has been added to
                                            // the string

            TimeSpan? diffBetweenNowAndDue = date2 - date1;

            if (diffBetweenNowAndDue.HasValue)
            {
                if (diffBetweenNowAndDue.Value.TotalSeconds < 60)
                {
                    if (shortVersion)
                        valueOutputted = true;

                    rValue += $"{(int)diffBetweenNowAndDue.Value.TotalSeconds} seconds";
                    needComma = true;
                }

                if (diffBetweenNowAndDue.Value.Days != 0)
                {
                    if (shortVersion)
                        valueOutputted = true;

                    rValue += $"{diffBetweenNowAndDue.Value.Days} day";
                    needComma = true;
                    if (diffBetweenNowAndDue.Value.Days > 1)
                        rValue += "s";
                }

                if ((diffBetweenNowAndDue.Value.Hours != 0) && (valueOutputted == false))
                {
                    if (shortVersion)
                        valueOutputted = true;

                    if (needComma)
                        rValue += ", ";
                    rValue += $"{diffBetweenNowAndDue.Value.Hours} hour";
                    needComma = true;
                    if (diffBetweenNowAndDue.Value.Hours > 1)
                        rValue += "s";
                }

                if ((diffBetweenNowAndDue.Value.Minutes != 0) && (valueOutputted == false))
                {
                    if (shortVersion)
                        valueOutputted = true;

                    if (needComma)
                        rValue += ", ";
                    rValue += $"{diffBetweenNowAndDue.Value.Minutes} minute";
                    if (diffBetweenNowAndDue.Value.Minutes > 1)
                        rValue += "s";
                }
            }

            return rValue;
        }
    }
}
