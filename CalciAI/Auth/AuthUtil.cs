using CalciAI.Extentions;
using CalciAI.Helpers;
using CalciAI.Models;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalciAI.Auth
{
    public static class AuthUtil
    {
        public static ProcessResult<TokenData> DecodeJwtToken(string token, string secret, bool verify = true)
        {
            try
            {
                var secretKeyBytes = Encoding.UTF8.GetBytes(secret);

                var builder = new JwtBuilder();
                var algorithm = new HMACSHA256Algorithm();

                var jwt = builder.WithAlgorithm(algorithm)
                            .WithSecret(secretKeyBytes)
                            .WithVerifySignature(verify)
                            .Decode<Dictionary<string, object>>(token);

                if (((long)jwt["exp"]) < (SystemTime.UnixNow() + 1))
                {
                    return ProcessResult<TokenData>.Fail(new Error("Token", "token expired"));
                }

                return ProcessResult<TokenData>.Success(new TokenData
                {
                    Username = jwt["sub"].ToString(),
                    Role = jwt["role"].ToString(),
                    Expiration = ((long)jwt["exp"]).FromUnixtime(),
                    IpAddress = jwt["ip"].ToString()
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"INVALID: {token}. ERROR: {e.Message}");
                return ProcessResult<TokenData>.Fail(new Error("Token", e.Message));
            }
        }

        public static string EncodeToken(TokenData payload, string secret)
        {
            var secretKeyBytes = Encoding.UTF8.GetBytes(secret);

            var builder = new JwtBuilder();
            var algorithm = new HMACSHA256Algorithm();

            var token = builder.WithAlgorithm(algorithm)
                                .WithSecret(secretKeyBytes)
                                .AddClaim("sub", payload.Username)
                                .AddClaim("role", payload.Role)
                                .AddClaim("exp", payload.Expiration.ToUnixtime())
                                .AddClaim("ip", payload.IpAddress)
                                .Encode();
            return token;
        }
    }
}
