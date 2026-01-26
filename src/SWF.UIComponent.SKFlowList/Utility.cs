using System;

namespace SWF.UIComponent.SKFlowList
{
    internal static class Utility
    {
        /// <summary>
        /// 小数点以下切捨て
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public static int ToRoundDown(float dValue)
        {
            var dCoef = Math.Pow(10, 0);
            var result = dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef : Math.Ceiling(dValue * dCoef) / dCoef;
            return (int)result;
        }

        /// <summary>
        /// 小数点以下切上げ
        /// </summary>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public static int ToRoundUp(float dValue)
        {
            var dCoef = Math.Pow(10, 0);
            var result = dValue > 0 ? Math.Ceiling(dValue * dCoef) / dCoef : Math.Floor(dValue * dCoef) / dCoef;
            return (int)result;
        }
    }
}
