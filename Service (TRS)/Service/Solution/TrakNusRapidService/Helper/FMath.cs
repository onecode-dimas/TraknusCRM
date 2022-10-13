using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TrakNusRapidService.Helper
{
    public class FMath
    {
        #region Constants
        private const string _classname = "FMath";
        #endregion

        public decimal Truncate(decimal value, int length)
        {
            try
            {
                char[] separator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator.ToCharArray();
                string[] temp = value.ToString().Split(separator[0]);
                if (temp.Length > 1)
                {
                    if (temp[1].Length < length)
                        temp[1] = temp[1].Substring(0, temp[1].Length);
                    else
                        temp[1] = temp[1].Substring(0, length);
                    return Convert.ToDecimal(temp[0] + separator[0] + temp[1]);
                }
                else
                {
                    return Convert.ToDecimal(temp[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Truncate : " + ex.Message);
            }
        }

        public decimal RoundModeUp(decimal value, int length)
        {
            try
            {
                decimal truncate = Truncate(value, length);
                if (value > truncate)
                {
                    double point = 1 / Math.Pow(10, length);
                    value = truncate + (decimal)point;
                }
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RoundModeUp : " + ex.Message);
            }
        }

        public decimal RoundUp(decimal value)
        {
            string tot = string.Empty;
            char[] separator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator.ToCharArray();
            string[] temp = Math.Round(value, 2).ToString().Split(separator[0]);
            if (temp.Length > 1)
            {
                string sValue = temp[1].Substring(0, 1);
                int iValue = Convert.ToInt16(sValue) + 1;
                tot = temp[0].ToString() + "." + iValue.ToString();
            }
            return Convert.ToDecimal(tot);
        }
    }
}
