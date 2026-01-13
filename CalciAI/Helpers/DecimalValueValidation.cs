using System;

namespace CalciAI.Helpers
{
    public static class DecimalValueValidation
    {
        public static bool ValidateDecimal_18_02(decimal? value)
        {
            if (value > 0)
            {
                var len = value.ToString().Length - 1;
                if(len > 18)
                    return false;
            }

            return true;
        }
    }
}