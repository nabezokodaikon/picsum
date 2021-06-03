using System;

namespace SWF.UIComponent.FlowList
{
    internal static class Utility
    {
        /// <summary>
        /// 小数点以下切捨て
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public static int ToRoundDown(double dValue)
        {
            double dCoef = Math.Pow(10, 0);
            double result = dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef : Math.Ceiling(dValue * dCoef) / dCoef;
            return (int)result;
        }

        /// <summary>
        /// 小数点以下切上げ
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public static int ToRoundUp(double dValue)
        {
            double dCoef = Math.Pow(10, 0);
            double result = dValue > 0 ? Math.Ceiling(dValue * dCoef) / dCoef : Math.Floor(dValue * dCoef) / dCoef;
            return (int)result;
        }
    }
}
