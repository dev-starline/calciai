using CalciAI.Caching;
using CalciAI.Crypto;
using CalciAI.Models;
using StackExchange.Redis;
using System;


namespace CalciAI.Auth
{
    /// <summary>
    /// NOTE: Used RedisBus to have common token service between Operator and Master
    /// We can think of better approach in future.
    /// </summary>
    public static class TokenService
    {
        public static string CreateTokenWithExpiration(OperatorUserId operatorPlayerId, int expirationInSeconds)
        {
            string token = Randomize.CreateSecureRandomString();
            RedisBus.RedisCache.StringSet($"AUTH.{token}", operatorPlayerId.ToString(), TimeSpan.FromSeconds(expirationInSeconds));
            return token;
        }

        /// <summary>
        /// This will validate token till it get expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ProcessResult<OperatorUserId> ValidateToken(string token)
        {
            var userVal = RedisBus.RedisCache.StringGet($"AUTH.{token}");

            if(userVal == RedisValue.Null)
            {
                return ProcessResult<OperatorUserId>.Fail("token", "is invalid or expired");
            }

            var operatorUserId = new OperatorUserId(userVal);

            return ProcessResult<OperatorUserId>.Success(operatorUserId);
        }

        /// <summary>
        /// This will validate token and remove from store
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ProcessResult<OperatorUserId> ValidateOneTimeToken(string token)
        {
            var userVal = RedisBus.RedisCache.StringGet($"AUTH.{token}");

            if (userVal == RedisValue.Null)
            {
                return ProcessResult<OperatorUserId>.Fail("token", "is invalid or expired");
            }

            var operatorUserId = new OperatorUserId(userVal);

            return ProcessResult<OperatorUserId>.Success(operatorUserId);
        }

        /// <summary>
        /// This will validate token and add new expiry
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ProcessResult<OperatorUserId> ValidateSlidingToken(string token, int expirationInSeconds)
        {
            var userVal = RedisBus.RedisCache.StringGetSetExpiry($"AUTH.{token}", TimeSpan.FromSeconds(expirationInSeconds));

            if (userVal == RedisValue.Null)
            {
                return ProcessResult<OperatorUserId>.Fail("token", "is invalid or expired");
            }

            var operatorUserId = new OperatorUserId(userVal);

            return ProcessResult<OperatorUserId>.Success(operatorUserId);
        }
    }
}
