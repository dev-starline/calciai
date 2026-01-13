using CalciAI.Models;
using System.Reflection;

namespace CalciAI
{
    public static class ErrorProvider
    {
        public static Error BAD_REQUEST_MISSING_INFO(string parameter) => new("BAD_REQUEST", $"{parameter} required");

        public static Error ACCOUNT_BLOCKED => new("ACCOUNT_BLOCKED", "ACCOUNT BLOCK");
        public static Error BANNED_SCRIPT => new("BANNED_SCRIPT", "CLOSE ONLY");
        public static Error ACCOUNT_CLOSE_ONLY_BLOCKED => new("ACCOUNT_CLOSE_ONLY_BLOCKED ", "CLOSE ONLY");
        public static Error LIMIT_PLACED_BW_HIGH_LOW => new("LIMIT_PLACED_BW_HIGH_LOW", "INVALID PRICE");

        public static Error INVALID_USER => new("INVALID_USER", "INVALID USER");

        public static Error TRADE_TIME_CLOSED => new("TRADE_TIME_CLOSED", "MARKET CLOSE");

        public static Error ClOSE_ONLY_ALLOWED => new("ClOSE_ONLY_ALLOWED", "ClOSE ONLY ALLOWED");
        public static Error ROLL_OVER_RUNNING => new("ROLL_OVER_RUNNING", "ROLLOVER IN PROGRESS");

        public static Error SYMBOL_NOT_FOUND => new("SYMBOL_NOT_FOUND", "SYMBOL NOT FOUND");
        public static Error SYMBOL_CONFIG_NOT_FOUND => new("SYMBOL_CONFIG_NOT_FOUND", "SYMBOL CONFIG NOT FOUND");

        public static Error SYMBOL_TRADE_DISABLED => new("SYMBOL_TRADE_DISABLED", "TRADE DISABLE");
        public static Error DEALER_TRADE_LOCK => new("DEALER_TRADE_LOCK", "TRADE LOCK");
        public static Error SYMBOL_OFF_ODDS => new("SYMBOL_OFF_ODDS", "OFF ODDS");

        public static Error INVALID_PRICE => new("INVALID_PRICE", "INVALID PRICE");
        public static Error INVALID_SIDE => new("INVALID_SIDE", "INVALID TRADE TYPE");
        public static Error CLIENT_POSITION_CLOSE_IN_HOLD_TIME(int time) => new("CLIENT_POSITION_CLOSE_IN_HOLD_TIME", $"Trade Hold Time {time} Sec");
        public static Error CROSS_LIMIT_RESTRICTED_IN_HOLD_TIME(int time) => new("CROSS_LIMIT_RESTRICTED_IN_HOLD_TIME", $"Trade Hold Time {time} Sec");
        public static Error SECURITY_DISABLED => new("SECURITY_DISABLED", "DISABLE");
        public static Error SYMBOL_ALLOWED_FOR(string type) => new("SYMBOL_ALLOWED_FOR", $"ALLOW OF {type}");
        public static Error ORDER_TYPE_NOT_ALLOWED(string type) => new("ORDER_TYPE_NOT_ALLOWED", $"{type} NOT ALLOW ");
        public static Error ORDER_NOT_FOUND => new("ORDER_NOT_FOUND", "NO DATA FOUND");
        public static Error SYMBOL_ADVANCE_LIMIT_RESTRICTED => new("SYMBOL_ADVANCE_LIMIT_RESTRICTED", "ADVANCE LIMIT REJECTED");

        public static Error INVALID_MARKET_ID => new("INVALID_MARKET_ID", "INVALID PERAMETER");

        public static Error ID_REQUIRED => new("ID_REQUIRED", "INVALID PERAMETER");

        public static Error BET_LOCKED => new("BET_LOCKED", "TRADE DISABLE");

        public static Error SYMBOL_EXPIRED => new("SYMBOL_EXPIRED", "EXPIRED");

        public static Error OPEN_POSITION_NOT_FOUND => new("OPEN_POSITION_NOT_FOUND", "POSITION IS NOT OPEN");
        public static Error MIN_VOLUME_LIMIT(double limit) => new("MIN_VOLUME_LIMIT", $"MINIMUM {limit} ALLOW");

        public static Error STEP_VOLUME_MISMATCHED(double limit) => new("STEP_VOLUME_MISMATCHED", $"STEP {limit} ALLOW");

        public static Error MAX_LIMIT_EXCEED(double limit) => new("MAX_LIMIT_EXCEED", $"MAX {limit} ALLOW");

        public static Error ONE_CLICK_EXCEED(double limit) => new("ONE_CLICK_EXCEED", $"ONE CLICK {limit} ALLOW");

        public static Error INSUFFICIENT_FUNDS(double balance) => new("INSUFFICIENT_FUNDS", $"NO MONEY");

        public static Error Parse(string code)
        {
            var property = typeof(ErrorProvider).GetProperty(code, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (property == null)
            {
                return new Error();
            }

            return (Error)property.GetValue(null);
        }
    }
}
