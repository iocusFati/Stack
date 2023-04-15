using System;

namespace Utils
{
    public static class MyMath
    {
        public static float FindProportion(float x1, float x2, float y2) => 
            x1 * y2 / x2;

        public static bool SignToBool(NumSign isNegative)
        {
            return isNegative switch
            {
                Utils.NumSign.Negative => false,
                Utils.NumSign.Positive => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static NumSign NumSign(float num) => 
            num < 0 ? Utils.NumSign.Positive : Utils.NumSign.Negative;

        public static NumSign NegateSign(NumSign sign) =>
            sign switch
            {
                Utils.NumSign.Negative => Utils.NumSign.Positive,
                Utils.NumSign.Positive => Utils.NumSign.Negative,
                _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, null)
            };

        public static int BoolToInt(bool boolean) =>
            boolean ? 1 : -1;
    }
}